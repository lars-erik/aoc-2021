using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.JSInterop;

namespace blazorgui.Models
{
    public class CommonShapes
    {
        private Canvas2DContext context;
        private WindowDimension dimensions;
        public IJSRuntime runtime;

        public CommonShapes(Canvas2DContext context, WindowDimension dimensions, IJSRuntime runtime)
        {
            this.context = context;
            this.dimensions = dimensions;
            this.runtime = runtime;
        }

        public async Task DrawBackground()
        {
            var gdt = await runtime.InvokeAsync<IJSInProcessObjectReference>(
                "createLinearGradient",
                context,
                0, 0, 0, dimensions.Height,
                new[]
                {
                new object[] {0, "blue"},
                new object[] {1, "black"},
                }
                );

            await context.SetFillStyleAsync(gdt);
            await context.FillRectAsync(0, 0, dimensions.Width, dimensions.Height);
        }

        public async Task DrawSubmarine()
        {
            int subX = 0;
            int subY = 0;

            var bodyGrad = await runtime.InvokeAsync<IJSInProcessObjectReference>(
                "createLinearGradient",
                context,
                0, subY - 50, 0, subY + 50,
                new[]
                {
                new object[] {0, "yellow"},
                new object[] {.4, "#FFFFBB"},
                new object[] {.6, "#FFFFBB"},
                new object[] {1, "yellow"}
                }
                );


            await context.SetStrokeStyleAsync("black");
            await context.SetLineWidthAsync(2);

            await context.BeginPathAsync();
            await context.RectAsync(subX - 149, subY - 50, 150, 100);
            await context.SetFillStyleAsync(bodyGrad);
            await context.FillAsync();
            await context.StrokeAsync();

            var frontGrad = await runtime.InvokeAsync<IJSInProcessObjectReference>(
                "createRadialGradient",
                context,
                subX, subY, 10, subX, subY, 50,
                new[]
                {
                new object[] {0, "#FFFFBB"},
                new object[] {1, "yellow"}
                }
                );

            await context.BeginPathAsync();
            await context.SetFillStyleAsync(frontGrad);
            await context.SetStrokeStyleAsync("black");
            await context.ArcAsync(
                subX,
                subY,
                50,
                Math.PI / 2,
                Math.PI / -2,
                true
                );
            await context.FillAsync();
            await context.StrokeAsync();
        }

}
}
