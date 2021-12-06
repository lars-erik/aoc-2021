using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using blazorgui.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace blazorgui.Components
{
    public abstract class CanvasComponentBase : ComponentBase
    {
        protected BECanvasComponent canvasReference;
        protected Canvas2DContext context;
        protected WindowDimension dimensions;
        protected CommonShapes commonShapes; 

        [Inject]
        protected IJSRuntime runtime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dimensions = await runtime.InvokeAsync<WindowDimension>("containerSize");
                context = await canvasReference.CreateCanvas2DAsync();
                commonShapes = new CommonShapes(context, dimensions, runtime);

                await DrawInitialCanvas();

                await runtime.InvokeVoidAsync("init", DotNetObjectReference.Create(this));

                StateHasChanged();

                await base.OnAfterRenderAsync(firstRender);
            }
        }

        protected virtual async Task DrawInitialCanvas()
        {
            await Task.CompletedTask;
        }

        public abstract ValueTask Update(double timeStamp);
    }
}
