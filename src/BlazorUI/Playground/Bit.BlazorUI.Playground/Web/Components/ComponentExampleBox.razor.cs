﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Bit.BlazorUI.Playground.Web.Components;

public partial class ComponentExampleBox
{
    private bool showCode;

    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }

    [Parameter] public string Title { get; set; }
    [Parameter] public string ExampleId { get; set; }
    [Parameter] public string HTMLSourceCode { get; set; }
    [Parameter] public string CSharpSourceCode { get; set; }
    [Parameter] public RenderFragment ExamplePreview { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JSRuntime.InvokeVoidAsync("highlightSnippet");
    }

    private async Task CopyCodeToClipboard()
    {
        await JSRuntime.CopyToClipboard(HTMLSourceCode + CSharpSourceCode);
    }

    private async Task CopyLinkToClipboard()
    {
        var currentUrl = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "/", StringComparison.Ordinal);
        currentUrl = currentUrl.Contains("#") ? currentUrl.Substring(0, currentUrl.IndexOf("#")) : currentUrl;
        var exampleUrl = $"{currentUrl}#{ExampleId}";
        await JSRuntime.CopyToClipboard(exampleUrl);
    }
}
