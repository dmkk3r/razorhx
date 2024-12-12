using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorHx.Components;

public class HxComponent : IComponent
{
    private readonly RenderFragment _renderFragment;
    private bool _hasNeverRendered = true;
    private bool _hasPendingQueuedRender;
    private bool _initialized;
    private RenderHandle _renderHandle;

    public HxComponent()
    {
        _renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            _hasNeverRendered = false;
            BuildRenderTree(builder);
        };
    }

    public void Attach(RenderHandle renderHandle)
    {
        if (_renderHandle.IsInitialized)
            throw new InvalidOperationException(
                $"The render handle is already set. Cannot initialize a {nameof(ComponentBase)} more than once.");

        _renderHandle = renderHandle;
    }

    /// <summary>
    ///     Sets parameters supplied by the component's parent in the render tree.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A <see cref="Task" /> that completes when the component has finished updating and rendering itself.</returns>
    /// <remarks>
    ///     <para>
    ///         Parameters are passed when <see cref="SetParametersAsync(ParameterView)" /> is called. It is not required that
    ///         the caller supply a parameter value for all of the parameters that are logically understood by the component.
    ///     </para>
    ///     <para>
    ///         The default implementation of <see cref="SetParametersAsync(ParameterView)" /> will set the value of each
    ///         property
    ///         decorated with <see cref="ParameterAttribute" /> or <see cref="CascadingParameterAttribute" /> that has
    ///         a corresponding value in the <see cref="ParameterView" />. Parameters that do not have a corresponding value
    ///         will be unchanged.
    ///     </para>
    /// </remarks>
    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (_initialized) return CallOnParametersSetAsync();
        _initialized = true;

        return RunInitAndSetParametersAsync();
    }

    protected virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Developers can either override this method in derived classes, or can use Razor
        // syntax to define a derived class and have the compiler generate the method.

        // Other code within this class should *not* invoke BuildRenderTree directly,
        // but instead should invoke the _renderFragment field.
    }

    /// <summary>
    ///     Method invoked when the component is ready to start, having received its
    ///     initial parameters from its parent in the render tree.
    /// </summary>
    protected virtual void OnInitialized()
    {
    }

    /// <summary>
    ///     Method invoked when the component is ready to start, having received its
    ///     initial parameters from its parent in the render tree.
    ///     Override this method if you will perform an asynchronous operation and
    ///     want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    protected virtual Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Method invoked when the component has received parameters from its parent in
    ///     the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    protected virtual void OnParametersSet()
    {
    }

    /// <summary>
    ///     Method invoked when the component has received parameters from its parent in
    ///     the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
    protected virtual Task OnParametersSetAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Notifies the component that its state has changed. When applicable, this will
    ///     cause the component to be re-rendered.
    /// </summary>
    protected void StateHasChanged()
    {
        if (_hasPendingQueuedRender) return;

        if (!_hasNeverRendered && !ShouldRender() && !_renderHandle.IsRenderingOnMetadataUpdate) return;

        _hasPendingQueuedRender = true;

        try
        {
            _renderHandle.Render(_renderFragment);
        }
        catch
        {
            _hasPendingQueuedRender = false;
            throw;
        }
    }

    /// <summary>
    ///     Returns a flag to indicate whether the component should render.
    /// </summary>
    /// <returns></returns>
    protected virtual bool ShouldRender()
    {
        return true;
    }

    /// <summary>
    ///     Executes the supplied work item on the associated renderer's
    ///     synchronization context.
    /// </summary>
    /// <param name="workItem">The work item to execute.</param>
    protected Task InvokeAsync(Action workItem)
    {
        return _renderHandle.Dispatcher.InvokeAsync(workItem);
    }

    /// <summary>
    ///     Executes the supplied work item on the associated renderer's
    ///     synchronization context.
    /// </summary>
    /// <param name="workItem">The work item to execute.</param>
    protected Task InvokeAsync(Func<Task> workItem)
    {
        return _renderHandle.Dispatcher.InvokeAsync(workItem);
    }

    /// <summary>
    ///     Treats the supplied <paramref name="exception" /> as being thrown by this component. This will cause the
    ///     enclosing ErrorBoundary to transition into a failed state. If there is no enclosing ErrorBoundary,
    ///     it will be regarded as an exception from the enclosing renderer.
    ///     This is useful if an exception occurs outside the component lifecycle methods, but you wish to treat it
    ///     the same as an exception from a component lifecycle method.
    /// </summary>
    /// <param name="exception">The <see cref="Exception" /> that will be dispatched to the renderer.</param>
    /// <returns>A <see cref="Task" /> that will be completed when the exception has finished dispatching.</returns>
    protected Task DispatchExceptionAsync(Exception exception)
    {
        return _renderHandle.DispatchExceptionAsync(exception);
    }

    private async Task RunInitAndSetParametersAsync()
    {
        OnInitialized();
        var task = OnInitializedAsync();

        if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
        {
            // Call state has changed here so that we render after the sync part of OnInitAsync has run
            // and wait for it to finish before we continue. If no async work has been done yet, we want
            // to defer calling StateHasChanged up until the first bit of async code happens or until
            // the end. Additionally, we want to avoid calling StateHasChanged if no
            // async work is to be performed.
            StateHasChanged();

            try
            {
                await task;
            }
            catch // avoiding exception filters for AOT runtime support
            {
                // Ignore exceptions from task cancellations.
                // Awaiting a canceled task may produce either an OperationCanceledException (if produced as a consequence of
                // CancellationToken.ThrowIfCancellationRequested()) or a TaskCanceledException (produced as a consequence of awaiting Task.FromCanceled).
                // It's much easier to check the state of the Task (i.e. Task.IsCanceled) rather than catch two distinct exceptions.
                if (!task.IsCanceled) throw;
            }

            // Don't call StateHasChanged here. CallOnParametersSetAsync should handle that for us.
        }

        await CallOnParametersSetAsync();
    }

    private Task CallOnParametersSetAsync()
    {
        OnParametersSet();
        var task = OnParametersSetAsync();

        // If no async work is to be performed, i.e. the task has already ran to completion
        // or was canceled by the time we got to inspect it, avoid going async and re-invoking
        // StateHasChanged at the culmination of the async work.
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                              task.Status != TaskStatus.Canceled;

        // We always call StateHasChanged here as we want to trigger a rerender after OnParametersSet and
        // the synchronous part of OnParametersSetAsync has run.
        StateHasChanged();

        return shouldAwaitTask ? CallStateHasChangedOnAsyncCompletion(task) : Task.CompletedTask;
    }

    private async Task CallStateHasChangedOnAsyncCompletion(Task task)
    {
        try
        {
            await task;
        }
        catch // avoiding exception filters for AOT runtime support
        {
            // Ignore exceptions from task cancellations, but don't bother issuing a state change.
            if (task.IsCanceled) return;

            throw;
        }

        StateHasChanged();
    }
}