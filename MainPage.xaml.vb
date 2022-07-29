Imports Windows.ApplicationModel.DataTransfer
Imports Windows.ApplicationModel.Email
Imports Windows.Foundation.Metadata
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI
Imports Windows.UI.Text
Imports Windows.UI.ViewManagement.Core

Public NotInheritable Class MainPage
    Inherits Page
    ''Variables
    Private FormatChanged As Boolean = False
    Private ReadOnly FoundKeys As New List(Of ITextRange)()
    Private HighlightColor As Color = Color.FromArgb(255, 150, 190, 255)
    Private OldQuery As String = String.Empty
    Private ReadOnly Timer As DispatcherTimer
    Public Sub New()
        InitializeComponent()
        Timer = New DispatcherTimer() With {.Interval = TimeSpan.FromSeconds(5)}
        AddHandler Timer.Tick, AddressOf Timer_Tick
        Timer.Start()
    End Sub
    ''CapitalizationFlyout subroutines
    Private Sub AllCapsItem_Click(sender As Object, e As RoutedEventArgs) Handles AllCapsItem.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.AllCaps = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub LowercaseItem_Click(sender As Object, e As RoutedEventArgs) Handles LowercaseItem.Click
        WritingBox.Document.Selection.ChangeCase(0)
    End Sub
    Private Sub SubscriptItem_Click(sender As Object, e As RoutedEventArgs) Handles SubscriptItem.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.Subscript = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub SuperscriptItem_Click(sender As Object, e As RoutedEventArgs) Handles SuperscriptItem.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.Superscript = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub SmallCapsItem_Click(sender As Object, e As RoutedEventArgs) Handles SmallCapsItem.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.SmallCaps = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub UppercaseItem_Click(sender As Object, e As RoutedEventArgs) Handles UppercaseItem.Click
        WritingBox.Document.Selection.ChangeCase(1)
    End Sub
    ''ColorBar subroutines
    Private Sub ApplytoPageButton_Click(sender As Object, e As RoutedEventArgs) Handles ApplyToPageButton.Click
        WritingBox.Background = New SolidColorBrush(TextAndPageColorPicker.Color)
    End Sub
    Private Sub ApplytoTextButton_Click(sender As Object, e As RoutedEventArgs) Handles ApplyToTextButton.Click
        WritingBox.Document.Selection.CharacterFormat.ForegroundColor = TextAndPageColorPicker.Color
    End Sub
    Private Sub CloseColorMenuButton_Click(sender As Object, e As RoutedEventArgs) Handles CloseColorMenuButton.Click
        ApplyToTextButton.Visibility = 1
        ApplyToPageButton.Visibility = 1
        ColorBar.Visibility = 1
        TextAndPageColorMenuBackground.Visibility = 1
        TextAndPageColorPicker.Visibility = 1
    End Sub
    ''DataRequested and ShareSourceLoad subroutines
    Private Sub DataRequested(sender As DataTransferManager, e As DataRequestedEventArgs)
        Dim Request As DataRequest = e.Request
        Request.Data.SetText(WritingBox.Document.Selection.Text.ToString())
        Request.Data.Properties.Title = "Shared Fountain Text"
        Request.Data.Properties.Description = "This is a shared text selection from Fountain."
        Request.Data.Properties.ApplicationName = "Fountain"
    End Sub
    Private Sub ShareSourceLoad()
        Dim FountainDataTransferManager As DataTransferManager = DataTransferManager.GetForCurrentView()
        AddHandler FountainDataTransferManager.DataRequested, New TypedEventHandler(Of DataTransferManager, DataRequestedEventArgs)(AddressOf DataRequested)
        DataTransferManager.ShowShareUI()
    End Sub
    ''FountainCommandBar subroutines
    Private Async Sub AddImageButton_Click(sender As Object, e As RoutedEventArgs) Handles AddImageButton.Click
        Dim Open As New FileOpenPicker With {.SuggestedStartLocation = 6}
        Open.FileTypeFilter.Add(".gif")
        Open.FileTypeFilter.Add(".jpg")
        Open.FileTypeFilter.Add(".png")
        Dim File As StorageFile = Await Open.PickSingleFileAsync()
        If File IsNot Nothing Then
            Dim RandomAccessStream As IRandomAccessStream = Await File.OpenAsync(0)
            Dim Image As New BitmapImage()
            WritingBox.Document.Selection.InsertImage(Image.PixelWidth, Image.PixelHeight, 0, 1, File.DisplayName.ToString(), RandomAccessStream)
        End If
        If File Is Nothing Then
            Dim CloseCornerRadius As CornerRadius
            CloseCornerRadius.BottomLeft = 2
            CloseCornerRadius.BottomRight = 2
            CloseCornerRadius.TopLeft = 2
            CloseCornerRadius.TopRight = 2
            Dim DialogBorderThickness As Thickness
            DialogBorderThickness.Bottom = 0
            DialogBorderThickness.Left = 0
            DialogBorderThickness.Right = 0
            DialogBorderThickness.Top = 0
            Dim DialogCornerRadius As CornerRadius
            DialogCornerRadius.BottomLeft = 4
            DialogCornerRadius.BottomRight = 4
            DialogCornerRadius.TopLeft = 4
            DialogCornerRadius.TopRight = 4
            Dim CloseButtonStyle As New Style(GetType(Button))
            CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
            CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
            Dim OpenFileErrorDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "Sorry, the file could not be opened. Please put down your spear and try again.", .CornerRadius = DialogCornerRadius, .Title = "Open File Error"}
            Await OpenFileErrorDialog.ShowAsync()
        End If
    End Sub
    Private Async Sub AddToCalendarButton_Click(sender As Object, e As RoutedEventArgs) Handles AddToCalendarButton.Click
        Dim Appointment = New Appointments.Appointment With {.Reminder = TimeSpan.FromDays(1), .Sensitivity = 0, .Subject = WritingBox.Document.Selection.Text.ToString()}
        Dim Rect = New Rect(New Point(Window.Current.Bounds.Width / 2, Window.Current.Bounds.Height / 2), New Size())
        Dim AppointmentId As String = Await Appointments.AppointmentManager.ShowAddAppointmentAsync(Appointment, Rect, 0)
    End Sub
    Private Sub AlignCenterButton_Click(sender As Object, e As RoutedEventArgs) Handles AlignCenterButton.Click
        WritingBox.Document.Selection.ParagraphFormat.Alignment = 2
    End Sub
    Private Sub AlignLeftButton_Click(sender As Object, e As RoutedEventArgs) Handles AlignLeftButton.Click
        WritingBox.Document.Selection.ParagraphFormat.Alignment = 1
    End Sub
    Private Sub AlignRightButton_Click(sender As Object, e As RoutedEventArgs) Handles AlignRightButton.Click
        WritingBox.Document.Selection.ParagraphFormat.Alignment = 3
    End Sub
    Private Sub BoldButton_Click(sender As Object, e As RoutedEventArgs) Handles BoldButton.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.Bold = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub BulletButton_Click(sender As Object, e As RoutedEventArgs) Handles BulletButton.Click
        WritingBox.Document.Selection.ParagraphFormat.ListType = 2
    End Sub
    Private Sub ClipboardHistoryButton_Click(sender As Object, e As RoutedEventArgs) Handles ClipboardHistoryButton.Click
        CoreInputView.GetForCurrentView().TryShow(5)
    End Sub
    Private Sub ColorButton_Click(sender As Object, e As RoutedEventArgs) Handles ColorButton.Click
        ApplyToTextButton.Visibility = 0
        ApplyToPageButton.Visibility = 0
        ColorBar.Visibility = 0
        TextAndPageColorMenuBackground.Visibility = 0
        TextAndPageColorPicker.Visibility = 0
    End Sub
    Private Sub CopyButton_Click(sender As Object, e As RoutedEventArgs) Handles CopyButton.Click
        If WritingBox.Document.CanCopy() Then
            WritingBox.Document.Selection.Copy()
        End If
    End Sub
    Private Sub CutButton_Click(sender As Object, e As RoutedEventArgs) Handles CutButton.Click
        WritingBox.Document.Selection.Cut()
    End Sub
    Private Async Sub DefineButton_Click(sender As Object, e As RoutedEventArgs) Handles DefineButton.Click
        Dim MWURI As New Uri("https://www.merriam-webster.com/dictionary/" & WritingBox.Document.Selection.Text)
        Dim PromptOptions = New Windows.System.LauncherOptions With {.TreatAsUntrusted = False}
        Dim URISuccess1 = Await Windows.System.Launcher.LaunchUriAsync(MWURI, PromptOptions)
        If URISuccess1 Then
        Else
        End If
    End Sub
    Private Async Sub EmailButton_Click(sender As Object, e As RoutedEventArgs) Handles EmailButton.Click
        Dim FountainStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
        Dim FountainFile = Await FountainStorageFolder.GetFileAsync("Fountain Document.rtf")
        If FountainFile IsNot Nothing Then
            SendEmail(FountainFile)
        End If
    End Sub
    Private Sub EmojiButton_Click(sender As Object, e As RoutedEventArgs) Handles EmojiButton.Click
        CoreInputView.GetForCurrentView().TryShow(3)
    End Sub
    Private Sub HandwritingButton_Click(sender As Object, e As RoutedEventArgs) Handles HandwritingButton.Click
        CoreInputView.GetForCurrentView().TryShow(2)
    End Sub
    Private Sub ItalicizeButton_Click(sender As Object, e As RoutedEventArgs) Handles ItalicizeButton.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.Italic = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub KeyboardButton_Click(sender As Object, e As RoutedEventArgs) Handles KeyboardButton.Click
        CoreInputView.GetForCurrentView().TryShow(1)
    End Sub
    Private Async Sub OpenButton_Click(sender As Object, e As RoutedEventArgs) Handles OpenButton.Click
        Dim Open As New FileOpenPicker With {.SuggestedStartLocation = 0}
        Open.FileTypeFilter.Add(".rtf")
        Dim File As StorageFile = Await Open.PickSingleFileAsync()
        If File IsNot Nothing Then
            Dim RandomAccessStream As IRandomAccessStream = Await File.OpenAsync(0)
            WritingBox.Document.LoadFromStream(8192, RandomAccessStream)
            DocumentTitleBlock.Text = File.DisplayName & ", opened at " & Date.Now.ToString("h:mm tt")
        End If
        If File Is Nothing Then
            Dim CloseCornerRadius As CornerRadius
            CloseCornerRadius.BottomLeft = 2
            CloseCornerRadius.BottomRight = 2
            CloseCornerRadius.TopLeft = 2
            CloseCornerRadius.TopRight = 2
            Dim DialogBorderThickness As Thickness
            DialogBorderThickness.Bottom = 0
            DialogBorderThickness.Left = 0
            DialogBorderThickness.Right = 0
            DialogBorderThickness.Top = 0
            Dim DialogCornerRadius As CornerRadius
            DialogCornerRadius.BottomLeft = 4
            DialogCornerRadius.BottomRight = 4
            DialogCornerRadius.TopLeft = 4
            DialogCornerRadius.TopRight = 4
            Dim CloseButtonStyle As New Style(GetType(Button))
            CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
            CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
            Dim OpenFileErrorDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "Sorry, the file could not be opened. Please put down your spear and try again.", .CornerRadius = DialogCornerRadius, .Title = "Open File Error"}
            Await OpenFileErrorDialog.ShowAsync()
        End If
    End Sub
    Private Sub PasteButton_Click(sender As Object, e As RoutedEventArgs) Handles PasteButton.Click
        If WritingBox.Document.CanPaste() Then
            WritingBox.Document.Selection.Paste(0)
        End If
    End Sub
    Private Sub RedoButton_Click(sender As Object, e As RoutedEventArgs) Handles RedoButton.Click
        WritingBox.Document.Redo()
    End Sub
    Private Async Sub SaveButton_Click(sender As Object, e As RoutedEventArgs) Handles SaveButton.Click
        Dim SavePicker As New FileSavePicker With {.SuggestedStartLocation = 0}
        SavePicker.FileTypeChoices.Add("Rich Text Format File (.rtf)", New List(Of String)() From {".rtf"})
        SavePicker.SuggestedFileName = "Fountain Document"
        Dim FountainFile As StorageFile = Await SavePicker.PickSaveFileAsync()
        If FountainFile IsNot Nothing Then
            CachedFileManager.DeferUpdates(FountainFile)
            Dim RandomAccessStream As IRandomAccessStream = Await FountainFile.OpenAsync(1)
            WritingBox.Document.SaveToStream(8192, RandomAccessStream)
            DocumentTitleBlock.Text = FountainFile.DisplayName & ", saved at " & Date.Now.ToString("h:mm tt")
            Dim status As Provider.FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(FountainFile)
            If status <> 1 Then
                Dim CloseCornerRadius As CornerRadius
                CloseCornerRadius.BottomLeft = 2
                CloseCornerRadius.BottomRight = 2
                CloseCornerRadius.TopLeft = 2
                CloseCornerRadius.TopRight = 2
                Dim DialogBorderThickness As Thickness
                DialogBorderThickness.Bottom = 0
                DialogBorderThickness.Left = 0
                DialogBorderThickness.Right = 0
                DialogBorderThickness.Top = 0
                Dim DialogCornerRadius As CornerRadius
                DialogCornerRadius.BottomLeft = 4
                DialogCornerRadius.BottomRight = 4
                DialogCornerRadius.TopLeft = 4
                DialogCornerRadius.TopRight = 4
                Dim CloseButtonStyle As New Style(GetType(Button))
                CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
                CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
                Dim SaveFileErrorDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "The file '" & FountainFile.DisplayName & "' could not be saved.", .CornerRadius = DialogCornerRadius, .Title = "Saving File Error"}
                Await SaveFileErrorDialog.ShowAsync()
            End If
        End If
    End Sub
    Private Async Sub SearchBingButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchBingButton.Click
        Dim BingURI As New Uri("https://www.bing.com/search?q=" & WritingBox.Document.Selection.Text)
        Dim PromptOptions = New Windows.System.LauncherOptions With {.TreatAsUntrusted = False}
        Dim URISuccess1 = Await Windows.System.Launcher.LaunchUriAsync(BingURI, PromptOptions)
        If URISuccess1 Then
        Else
        End If
    End Sub
    Private Sub ShareButton_Click(sender As Object, e As RoutedEventArgs) Handles ShareButton.Click
        ShareSourceLoad()
    End Sub
    Private Sub StrikethroughButton_Click(sender As Object, e As RoutedEventArgs) Handles StrikethroughButton.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            FountainCharacterFormatting.Strikethrough = 2
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub UnderlineButton_Click(sender As Object, e As RoutedEventArgs) Handles UnderlineButton.Click
        Dim FountainSelectedText As ITextSelection = WritingBox.Document.Selection
        If FountainSelectedText IsNot Nothing Then
            Dim FountainCharacterFormatting As ITextCharacterFormat = FountainSelectedText.CharacterFormat
            If FountainCharacterFormatting.Underline = 1 Then
                FountainCharacterFormatting.Underline = 2
            Else
                FountainCharacterFormatting.Underline = 1
            End If
            FountainSelectedText.CharacterFormat = FountainCharacterFormatting
        End If
    End Sub
    Private Sub UndoButton_Click(sender As Object, e As RoutedEventArgs) Handles UndoButton.Click
        WritingBox.Document.Undo()
    End Sub
    ''Navigation subroutines
    Protected Overrides Async Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        Dim CurrentInputPane As InputPane = InputPane.GetForCurrentView()
        AddHandler CurrentInputPane.Hiding, AddressOf OnHiding
        AddHandler CurrentInputPane.Showing, AddressOf OnShowing
        Try
            Dim FountainStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim FountainFile As StorageFile = Await FountainStorageFolder.CreateFileAsync("Fountain Document.rtf", 1)
            Dim RandomAccessStream As IRandomAccessStream = Await FountainFile.OpenAsync(1)
            If FountainFile IsNot Nothing Then
                CachedFileManager.DeferUpdates(FountainFile)
                WritingBox.Document.SaveToStream(8192, RandomAccessStream)
                DocumentTitleBlock.Text = FountainFile.DisplayName & ", autosaved at " & Date.Now.ToString("h:mm tt")
                Dim Status As Provider.FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(FountainFile)
                If Status <> 1 Then
                    Dim CloseCornerRadius As CornerRadius
                    CloseCornerRadius.BottomLeft = 2
                    CloseCornerRadius.BottomRight = 2
                    CloseCornerRadius.TopLeft = 2
                    CloseCornerRadius.TopRight = 2
                    Dim DialogBorderThickness As Thickness
                    DialogBorderThickness.Bottom = 0
                    DialogBorderThickness.Left = 0
                    DialogBorderThickness.Right = 0
                    DialogBorderThickness.Top = 0
                    Dim DialogCornerRadius As CornerRadius
                    DialogCornerRadius.BottomLeft = 4
                    DialogCornerRadius.BottomRight = 4
                    DialogCornerRadius.TopLeft = 4
                    DialogCornerRadius.TopRight = 4
                    Dim CloseButtonStyle As New Style(GetType(Button))
                    CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
                    CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
                    Dim SaveFileErrorDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "The selected file could not be saved.", .CornerRadius = DialogCornerRadius, .Title = "Saving File Error"}
                    Await SaveFileErrorDialog.ShowAsync()
                End If
            End If
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
    End Sub
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        Dim CurrentInputPane As InputPane = InputPane.GetForCurrentView()
        AddHandler CurrentInputPane.Hiding, AddressOf OnHiding
        AddHandler CurrentInputPane.Showing, AddressOf OnShowing
        Try
            Dim FountainStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim FountainFile = Await FountainStorageFolder.GetFileAsync("Fountain Document.rtf")
            If FountainFile IsNot Nothing Then
                Dim RandomAccessStream As IRandomAccessStream = Await FountainFile.OpenAsync(0)
                WritingBox.Document.LoadFromStream(8192, RandomAccessStream)
                DocumentTitleBlock.Text = FountainFile.DisplayName
            Else
                Dim CloseCornerRadius As CornerRadius
                CloseCornerRadius.BottomLeft = 2
                CloseCornerRadius.BottomRight = 2
                CloseCornerRadius.TopLeft = 2
                CloseCornerRadius.TopRight = 2
                Dim DialogBorderThickness As Thickness
                DialogBorderThickness.Bottom = 0
                DialogBorderThickness.Left = 0
                DialogBorderThickness.Right = 0
                DialogBorderThickness.Top = 0
                Dim DialogCornerRadius As CornerRadius
                DialogCornerRadius.BottomLeft = 4
                DialogCornerRadius.BottomRight = 4
                DialogCornerRadius.TopLeft = 4
                DialogCornerRadius.TopRight = 4
                Dim CloseButtonStyle As New Style(GetType(Button))
                CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
                CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
                Dim OpenFileErrorDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "The selected file could not be opened.", .CornerRadius = DialogCornerRadius, .Title = "Open File Error"}
                Await OpenFileErrorDialog.ShowAsync()
            End If
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
        SpellCheckSwitch.IsEnabled = True
        SpellCheckSwitch.IsOn = True
    End Sub
    ''OnHiding and OnShowing subroutines
    Private Sub OnHiding(sender As InputPane, e As InputPaneVisibilityEventArgs)
        KeyboardButton.Label = "Keyboard"
    End Sub
    Private Sub OnShowing(sender As InputPane, e As InputPaneVisibilityEventArgs)
        KeyboardButton.Label = "Showing keyboard"
    End Sub
    ''Search subroutines
    Private Async Sub FindText(query As String, Optional Isfocus As Boolean = False)
        If String.IsNullOrEmpty(query) Then Return
        If Not OldQuery.Equals(query) Then ResetTextFormat()
        OldQuery = query
        Dim DocumentText As String
        WritingBox.Document.GetText(0, DocumentText)
        Dim StartPosition = WritingBox.Document.Selection.EndPosition
        Dim EndPosition = DocumentText.Length
        Dim Range = WritingBox.Document.GetRange(StartPosition, EndPosition)
        Dim Result As Integer = Range.FindText(query, EndPosition - StartPosition, 0)
        FoundKeys.Add(Range)
        If Result = 0 Then
            WritingBox.Document.Selection.SetRange(0, 0)
            Dim CloseCornerRadius As CornerRadius
            CloseCornerRadius.BottomLeft = 2
            CloseCornerRadius.BottomRight = 2
            CloseCornerRadius.TopLeft = 2
            CloseCornerRadius.TopRight = 2
            Dim DialogBorderThickness As Thickness
            DialogBorderThickness.Bottom = 0
            DialogBorderThickness.Left = 0
            DialogBorderThickness.Right = 0
            DialogBorderThickness.Top = 0
            Dim DialogCornerRadius As CornerRadius
            DialogCornerRadius.BottomLeft = 4
            DialogCornerRadius.BottomRight = 4
            DialogCornerRadius.TopLeft = 4
            DialogCornerRadius.TopRight = 4
            Dim CloseButtonStyle As New Style(GetType(Button))
            CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
            CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
            Dim WordNotFoundDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "Sorry, that word was not found in the document.", .CornerRadius = DialogCornerRadius, .Title = "Word Not Found"}
            Await WordNotFoundDialog.ShowAsync()
        Else
            WritingBox.Document.Selection.SetRange(Range.StartPosition, Range.EndPosition)
            Range.CharacterFormat.BackgroundColor = HighlightColor
            FormatChanged = True
            Range.ScrollIntoView(0)
            If Isfocus Then WritingBox.Focus(2)
        End If
    End Sub
    Private Sub ResetTextFormat()
        If FormatChanged Then
            Dim RegularBackgroundColor = CType(WritingBox.Background, SolidColorBrush).Color
            For Each item In FoundKeys
                item.CharacterFormat.BackgroundColor = RegularBackgroundColor
            Next
            FormatChanged = False
            FoundKeys.Clear()
        End If
    End Sub
    ''SendEmail subroutine
    Private Async Sub SendEmail(file As StorageFile)
        Dim FountainEmailMessage As New EmailMessage With {.Subject = "Attached Document: " & file.Name, .Body = "Sent through Fountain"}
        Dim FountainAttachmentFile As StorageFile = file
        If FountainAttachmentFile IsNot Nothing Then
            Dim Stream As RandomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(FountainAttachmentFile)
            Dim Attachment As New EmailAttachment(FountainAttachmentFile.Name, Stream)
            FountainEmailMessage.Attachments.Add(Attachment)
        End If
        Await EmailManager.ShowComposeNewEmailAsync(FountainEmailMessage)
    End Sub
    ''SplitViewMenu subroutines
    Private Sub FontBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles FontBox.SelectionChanged
        If FontBox.SelectedIndex = 0 Then
            WritingBox.FontFamily = New FontFamily("Arial")
        End If
        If FontBox.SelectedIndex = 1 Then
            WritingBox.FontFamily = New FontFamily("Bahnschrift")
        End If
        If FontBox.SelectedIndex = 2 Then
            WritingBox.FontFamily = New FontFamily("Calibri")
        End If
        If FontBox.SelectedIndex = 3 Then
            WritingBox.FontFamily = New FontFamily("Cambria")
        End If
        If FontBox.SelectedIndex = 4 Then
            WritingBox.FontFamily = New FontFamily("Candara")
        End If
        If FontBox.SelectedIndex = 5 Then
            WritingBox.FontFamily = New FontFamily("Courier New")
        End If
        If FontBox.SelectedIndex = 6 Then
            WritingBox.FontFamily = New FontFamily("Gabriola")
        End If
        If FontBox.SelectedIndex = 7 Then
            WritingBox.FontFamily = New FontFamily("Georgia")
        End If
        If FontBox.SelectedIndex = 8 Then
            WritingBox.FontFamily = New FontFamily("Impact")
        End If
        If FontBox.SelectedIndex = 9 Then
            WritingBox.FontFamily = New FontFamily("Lucida Console")
        End If
        If FontBox.SelectedIndex = 10 Then
            WritingBox.FontFamily = New FontFamily("Segoe Script")
        End If
        If FontBox.SelectedIndex = 11 Then
            WritingBox.FontFamily = New FontFamily("Segoe UI")
        End If
        If FontBox.SelectedIndex = 12 Then
            WritingBox.FontFamily = New FontFamily("Tahoma")
        End If
        If FontBox.SelectedIndex = 13 Then
            WritingBox.FontFamily = New FontFamily("Times New Roman")
        End If
        If FontBox.SelectedIndex = 14 Then
            WritingBox.FontFamily = New FontFamily("Verdana")
        End If
    End Sub
    Private Sub FontButton_Click(sender As Object, e As RoutedEventArgs) Handles FontButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Sub FontSizeBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles FontSizeBox.SelectionChanged
        If FontSizeBox.SelectedIndex = 0 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 8
        End If
        If FontSizeBox.SelectedIndex = 1 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 9
        End If
        If FontSizeBox.SelectedIndex = 2 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 10
        End If
        If FontSizeBox.SelectedIndex = 3 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 11
        End If
        If FontSizeBox.SelectedIndex = 4 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 12
        End If
        If FontSizeBox.SelectedIndex = 5 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 14
        End If
        If FontSizeBox.SelectedIndex = 6 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 16
        End If
        If FontSizeBox.SelectedIndex = 7 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 18
        End If
        If FontSizeBox.SelectedIndex = 8 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 20
        End If
        If FontSizeBox.SelectedIndex = 9 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 22
        End If
        If FontSizeBox.SelectedIndex = 10 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 24
        End If
        If FontSizeBox.SelectedIndex = 11 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 26
        End If
        If FontSizeBox.SelectedIndex = 12 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 28
        End If
        If FontSizeBox.SelectedIndex = 13 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 30
        End If
        If FontSizeBox.SelectedIndex = 14 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 36
        End If
        If FontSizeBox.SelectedIndex = 15 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 48
        End If
        If FontSizeBox.SelectedIndex = 16 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 72
        End If
        If FontSizeBox.SelectedIndex = 17 Then
            WritingBox.Document.Selection.CharacterFormat.Size = 96
        End If
    End Sub
    Private Sub ListStyleBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListStyleBox.SelectionChanged
        If ListStyleBox.SelectedIndex = 0 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 3
        End If
        If ListStyleBox.SelectedIndex = 1 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 18
        End If
        If ListStyleBox.SelectedIndex = 2 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 17
        End If
        If ListStyleBox.SelectedIndex = 3 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 12
        End If
        If ListStyleBox.SelectedIndex = 4 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 10
        End If
        If ListStyleBox.SelectedIndex = 5 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 2
        End If
        If ListStyleBox.SelectedIndex = 6 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 9
        End If
        If ListStyleBox.SelectedIndex = 7 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 23
        End If
        If ListStyleBox.SelectedIndex = 8 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 24
        End If
        If ListStyleBox.SelectedIndex = 9 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 22
        End If
        If ListStyleBox.SelectedIndex = 10 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 4
        End If
        If ListStyleBox.SelectedIndex = 11 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 5
        End If
        If ListStyleBox.SelectedIndex = 12 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 19
        End If
        If ListStyleBox.SelectedIndex = 13 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 15
        End If
        If ListStyleBox.SelectedIndex = 14 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 16
        End If
        If ListStyleBox.SelectedIndex = 15 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 1
        End If
        If ListStyleBox.SelectedIndex = 16 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 6
        End If
        If ListStyleBox.SelectedIndex = 17 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 7
        End If
        If ListStyleBox.SelectedIndex = 18 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 13
        End If
        If ListStyleBox.SelectedIndex = 19 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 20
        End If
        If ListStyleBox.SelectedIndex = 20 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 21
        End If
        If ListStyleBox.SelectedIndex = 21 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 14
        End If
        If ListStyleBox.SelectedIndex = 22 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 8
        End If
        If ListStyleBox.SelectedIndex = 23 Then
            WritingBox.Document.Selection.ParagraphFormat.ListType = 11
        End If
    End Sub
    Private Sub ListStyleButton_Click(sender As Object, e As RoutedEventArgs) Handles ListStyleButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Sub MenuButton_Click(sender As Object, e As RoutedEventArgs) Handles MenuButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Async Sub SearchBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles SearchBox.KeyDown
        If e.Key = 13 Then
            Dim Query As String = SearchBox.Text
            FindText(Query, True)
            If String.IsNullOrEmpty(Query) Then
                Dim CloseCornerRadius As CornerRadius
                CloseCornerRadius.BottomLeft = 2
                CloseCornerRadius.BottomRight = 2
                CloseCornerRadius.TopLeft = 2
                CloseCornerRadius.TopRight = 2
                Dim DialogBorderThickness As Thickness
                DialogBorderThickness.Bottom = 0
                DialogBorderThickness.Left = 0
                DialogBorderThickness.Right = 0
                DialogBorderThickness.Top = 0
                Dim DialogCornerRadius As CornerRadius
                DialogCornerRadius.BottomLeft = 4
                DialogCornerRadius.BottomRight = 4
                DialogCornerRadius.TopLeft = 4
                DialogCornerRadius.TopRight = 4
                Dim CloseButtonStyle As New Style(GetType(Button))
                CloseButtonStyle.Setters.Add(New Setter(BorderThicknessProperty, DialogBorderThickness))
                CloseButtonStyle.Setters.Add(New Setter(CornerRadiusProperty, CloseCornerRadius))
                Dim EmptySearchBoxDialog As New ContentDialog() With {.BorderThickness = DialogBorderThickness, .CloseButtonStyle = CloseButtonStyle, .CloseButtonText = "Close", .Content = "Please enter an item into the search box.", .CornerRadius = DialogCornerRadius, .Title = "Search Box Empty"}
                Await EmptySearchBoxDialog.ShowAsync()
            End If
        End If
    End Sub
    Private Sub SearchBox_TextChanged(sender As Object, e As RoutedEventArgs) Handles SearchBox.TextChanged
        ResetTextFormat()
    End Sub
    Private Sub SearchButton_Click(sender As Object, e As RoutedEventArgs) Handles SearchButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Sub SettingsButton_Click(sender As Object, e As RoutedEventArgs) Handles SettingsButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
            SpellCheckSwitch.Visibility = 0
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
            SpellCheckSwitch.Visibility = 1
        End If
    End Sub
    Private Sub SpaceBetweenBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SpaceBetweenBox.SelectionChanged
        If SpaceBetweenBox.SelectedIndex = 0 Then
            WritingBox.Document.Selection.ParagraphFormat.SetLineSpacing(1, spacing:=1)
        End If
        If SpaceBetweenBox.SelectedIndex = 1 Then
            WritingBox.Document.Selection.ParagraphFormat.SetLineSpacing(2, spacing:=1)
        End If
        If SpaceBetweenBox.SelectedIndex = 2 Then
            WritingBox.Document.Selection.ParagraphFormat.SetLineSpacing(3, spacing:=1)
        End If
    End Sub
    Private Sub SpaceBetweenButton_Click(sender As Object, e As RoutedEventArgs) Handles SpaceBetweenButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    Private Sub SpellCheckSwitch_Toggled(sender As Object, e As RoutedEventArgs) Handles SpellCheckSwitch.Toggled
        If SpellCheckSwitch.IsOn = True Then
            WritingBox.IsSpellCheckEnabled = True
        End If
        If SpellCheckSwitch.IsOn = False Then
            WritingBox.IsSpellCheckEnabled = False
        End If
    End Sub
    Private Sub UnderlineBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles UnderlineBox.SelectionChanged
        If UnderlineBox.SelectedIndex = 0 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 6
        End If
        If UnderlineBox.SelectedIndex = 1 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 7
        End If
        If UnderlineBox.SelectedIndex = 2 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 8
        End If
        If UnderlineBox.SelectedIndex = 3 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 14
        End If
        If UnderlineBox.SelectedIndex = 4 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 15
        End If
        If UnderlineBox.SelectedIndex = 5 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 19
        End If
        If UnderlineBox.SelectedIndex = 6 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 5
        End If
        If UnderlineBox.SelectedIndex = 7 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 18
        End If
        If UnderlineBox.SelectedIndex = 8 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 4
        End If
        If UnderlineBox.SelectedIndex = 9 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 16
        End If
        If UnderlineBox.SelectedIndex = 10 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 17
        End If
        If UnderlineBox.SelectedIndex = 11 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 10
        End If
        If UnderlineBox.SelectedIndex = 12 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 2
        End If
        If UnderlineBox.SelectedIndex = 13 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 11
        End If
        If UnderlineBox.SelectedIndex = 14 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 9
        End If
        If UnderlineBox.SelectedIndex = 15 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 12
        End If
        If UnderlineBox.SelectedIndex = 16 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 13
        End If
        If UnderlineBox.SelectedIndex = 17 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 3
        End If
        If UnderlineBox.SelectedIndex = 18 Then
            WritingBox.Document.Selection.CharacterFormat.Underline = 1
        End If
    End Sub
    Private Sub UnderlineStyleButton_Click(sender As Object, e As RoutedEventArgs) Handles UnderlineStyleButton.Click
        Dim Back As New SymbolIcon With {.Symbol = 57618}
        Dim Open As New SymbolIcon With {.Symbol = 59136}
        SplitViewMenu.IsPaneOpen = Not SplitViewMenu.IsPaneOpen
        If SplitViewMenu.IsPaneOpen = True Then
            MenuButton.Content = Back
        End If
        If SplitViewMenu.IsPaneOpen = False Then
            MenuButton.Content = Open
        End If
    End Sub
    ''Timer subroutine
    Private Async Sub Timer_Tick(sender As Object, e As Object)
        Try
            Dim FountainStorageFolder As StorageFolder = ApplicationData.Current.LocalFolder
            Dim FountainFile As StorageFile = Await FountainStorageFolder.CreateFileAsync("Fountain Document.rtf", 1)
            Dim TextToGet As String
            WritingBox.Document.GetText(8292, TextToGet)
            Await FileIO.WriteTextAsync(FountainFile, TextToGet)
            DocumentTitleBlock.Text = FountainFile.DisplayName & ", autosaved at " & Date.Now.ToString("h:mm tt")
        Catch __unusedException1__ As Exception
            Debug.WriteLine("Sorry, the file was not found")
        End Try
    End Sub
    ''WritingBox subroutines
    Private Sub WritingBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles WritingBox.KeyDown
        If e.Key = 9 Then
            If WritingBox IsNot Nothing Then
                WritingBox.Document.Selection.TypeText(vbTab)
                e.Handled = True
            End If
        End If
    End Sub
    Private Sub WritingBox_SelectionChanged(sender As Object, e As RoutedEventArgs) Handles WritingBox.SelectionChanged
        FontBox.Text = WritingBox.Document.Selection.CharacterFormat.Name.ToString()
        FontSizeBox.Text = WritingBox.Document.Selection.CharacterFormat.Size.ToString()
        If FontBox.Text = "-9999999" Then
            FontBox.Text = "Mixed"
            FontBox.IsEditable = False
        Else
            FontBox.IsEditable = True
        End If
        If FontSizeBox.Text = "-9999999" Then
            FontSizeBox.Text = "Mixed"
            FontSizeBox.IsEditable = False
        Else
            FontSizeBox.IsEditable = True
        End If
    End Sub
    Private Sub WritingBox_TextChanged(sender As Object, e As RoutedEventArgs) Handles WritingBox.TextChanged
        Dim CanUndo As Boolean = WritingBox.Document.CanUndo()
        UndoButton.IsEnabled = CanUndo
        Dim CanRedo As Boolean = WritingBox.Document.CanRedo()
        RedoButton.IsEnabled = CanRedo
        If Not ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") Then
            If CanRedo Then
                RedoButton.IsEnabled = True
            Else
                RedoButton.IsEnabled = False
            End If
            If CanUndo Then
                UndoButton.IsEnabled = True
            Else
                UndoButton.IsEnabled = False
            End If
        End If
    End Sub
End Class