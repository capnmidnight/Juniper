﻿namespace Juniper.AppShell;

public interface IAppShell
{
    Task<Uri> GetSourceAsync();
    Task SetSourceAsync(Uri source);

    Task<string> GetTitleAsync();
    Task SetTitleAsync(string title);
    Task CloseAsync();
    Task WaitForCloseAsync();
}