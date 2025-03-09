using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client._Adventure.DiscordAuth;

public sealed partial class DiscordAuthLinkManager : IPostInjectInit
{
    [Dependency] private readonly IClientNetManager _net = default!;
    [Dependency] private readonly IUriOpener _uriOpener = default!;
    [Dependency] private readonly IClipboardManager _clipboard = null!;

    public void PostInject()
    {
        _net.ConnectFailed += OnConnectFailed;
    }

    public void PopupLink(string message, string link, string? title = null)
    {
        var popup = new DefaultWindow
        {
            Title = string.IsNullOrEmpty(title) ? Loc.GetString("popup-title") : title,
        };

        var messageLabel = new Label { Text = message };
        const int linkMaxSize = 50;
        var linkLabel = new Label {
            Text = link.Substring(0, Math.Min(link.Length, linkMaxSize)) +
            ((link.Length > linkMaxSize) ? "..." : string.Empty)
        };

        var vBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Vertical,
        };

        vBox.AddChild(messageLabel);
        vBox.AddChild(linkLabel);

        var copyButton = new Button
        {
            Text = "Копировать",
            HorizontalExpand = true,
        };

        var openButton = new Button
        {
            Text = "Открыть",
            HorizontalExpand = true,
        };

        copyButton.OnPressed += _ =>
        {
            _clipboard.SetText(link);
        };

        openButton.OnPressed += _ =>
        {
            _uriOpener.OpenUri(link);
        };

        var hBox = new BoxContainer
        {
            Orientation = BoxContainer.LayoutOrientation.Horizontal,
            HorizontalAlignment = Control.HAlignment.Right,
        };

        hBox.AddChild(openButton);
        hBox.AddChild(copyButton);
        vBox.AddChild(hBox);

        popup.Contents.AddChild(vBox);
        popup.OpenCentered();
    }

    public void OnConnectFailed(object? _, NetConnectFailArgs args)
    {
        object? returnedLink = args.Message.ValueOf("discord_auth_link");
        if (returnedLink is string discordAuthLink)
        {
            PopupLink(
                Loc.GetString("main-menu-failed-to-connect",("reason", args.Reason)),
                discordAuthLink,
                "Авторизация в Дискорде");
            return;
        }
        _net.ConnectFailed -= OnConnectFailed;
    }
}
