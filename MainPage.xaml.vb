Public NotInheritable Class MainPage
    'Navigation events
    Private Sub BackButton_Click(sender As Object, e As RoutedEventArgs) Handles BackButton.Click
        Navi.GoBack()
    End Sub
    Private Sub ForwardButton_Click(sender As Object, e As RoutedEventArgs) Handles ForwardButton.Click
        Navi.GoForward()
    End Sub
    Private Sub RefreshButton_Click(sender As Object, e As RoutedEventArgs) Handles RefreshButton.Click
        Navi.Refresh()
        RefreshButton.Visibility = Visibility.Collapsed
        StopButton.Visibility = Visibility.Visible
    End Sub
    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs) Handles StopButton.Click
        Navi.Stop()
        RefreshButton.Visibility = Visibility.Visible
        StopButton.Visibility = Visibility.Collapsed
    End Sub
    Private Sub HomeButton_Click(sender As Object, e As RoutedEventArgs) Handles HomeButton.Click
        Navi.Navigate(New Uri("ms-appx-web:///files/welcome.html"))
    End Sub
    Private Sub Navi_Loading(sender As Object, e As NavigationEventArgs) Handles Navi.Loading
        CustomTitleBar.Text = "Loading..."
        RefreshButton.Visibility = Visibility.Collapsed
        StopButton.Visibility = Visibility.Visible
    End Sub
    Private Sub Navi_LoadCompleted(sender As Object, e As NavigationEventArgs) Handles Navi.LoadCompleted
        SearchBox.Text = Navi.Source.ToString()
        HistoryBox.Items.Add(Navi.Source.ToString())
        CustomTitleBar.Text = Navi.DocumentTitle.ToString() & " • Fountain"
        RefreshButton.Visibility = Visibility.Visible
        StopButton.Visibility = Visibility.Collapsed
        If Navi.CanGoForward = True Then
            ForwardButton.IsEnabled = True
        Else
            ForwardButton.IsEnabled = False
        End If
        If Navi.CanGoBack = True Then
            BackButton.IsEnabled = True
        Else
            BackButton.IsEnabled = False
        End If
    End Sub
    Private Sub Navi_NewWindowRequested(ByVal sender As WebView, ByVal e As WebViewNewWindowRequestedEventArgs) Handles Navi.NewWindowRequested
        e.Handled = True
    End Sub
    Private Sub SearchBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles SearchBox.KeyDown
        If e.Key = Windows.System.VirtualKey.Enter Then
            Navi.Navigate(New Uri("https://www." & SearchBox.Text))
        End If
    End Sub
    Private Sub SearchBingButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchBingButton.Click
        Navi.Navigate(New Uri("https://www.bing.com/search?q=" & SearchBox.Text))
    End Sub
    'History and bookmarks
    Private Sub HistoryButton_Click(sender As Object, e As RoutedEventArgs) Handles HistoryButton.Click
        HistoryBox.Visibility = Visibility.Visible
    End Sub
    Private Sub GoBackButton_Click(sender As Object, e As RoutedEventArgs) Handles GoBackButton.Click
        HistoryBox.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BrowseTo_Click(sender As Object, e As RoutedEventArgs) Handles BrowseTo.Click
        Navi.Navigate(New Uri(HistoryBox.SelectedItem.ToString()))
        HistoryBox.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BookmarksButton_Copy_Click(sender As Object, e As RoutedEventArgs) Handles BookmarksButton_Copy.Click
        BookmarkBox.Visibility = Visibility.Visible
    End Sub
    Private Sub GoBackButton1_Click(sender As Object, e As RoutedEventArgs) Handles GoBackButton1.Click
        BookmarkBox.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BrowseTo1_Click(sender As Object, e As RoutedEventArgs) Handles BrowseTo1.Click
        Navi.Navigate(New Uri(BookmarkBox.SelectedItem.ToString()))
        BookmarkBox.Visibility = Visibility.Collapsed
    End Sub
    Private Sub BookmarksButton_Click(sender As Object, e As RoutedEventArgs) Handles BookmarksButton.Click
        BookmarkBox.Items.Add(Navi.Source.ToString())
    End Sub
End Class