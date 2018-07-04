using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.UI.Core;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using Windows.Storage.Search;
using Windows.UI.ViewManagement;
using Windows.System;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        CoreCursor buttonCursor = null;
        CoreCursor cursorBeforePointerEntered = null;
        


        public  MainPage()
        {
            
            this.InitializeComponent();
            Input.Text = " Open or Create new file....";
            Input.IsEnabled = false;
            buttonCursor = new CoreCursor(CoreCursorType.Hand, 0);
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            Window.Current.SetTitleBar(BackgroundElement);
        }
        public async Task<bool> Confirm(string content, string title, string ok, string cancel)
        {
            bool result = false;
            MessageDialog dialog = new MessageDialog(content, title);
            dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
            dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
            await dialog.ShowAsync();
            return result;
        }
        private async void New_Click(object sender, RoutedEventArgs e)
        {
            
            if (Input.IsEnabled&&await Confirm("Create New File?", "InLine18", "Yes", "No"))
            {
                Input.IsEnabled = true;
                Input.Text = string.Empty;
                Input.Select(Input.Text.Length, 0); 
            }
            else
            {
                Input.IsEnabled = true;
                Input.Text = string.Empty;
                Input.Select(Input.Text.Length, 0);
            }

            
            
        }
      
        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            Input.IsEnabled = true;
            Input.Text = "";
            try
            {
                 FileOpenPicker picker = new FileOpenPicker();
                 picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                 picker.FileTypeFilter.Add(".txt");
                 StorageFile file = await picker.PickSingleFileAsync();
              
                Input.Text = await FileIO.ReadTextAsync(file);
                Input.Select(Input.Text.Length, 0);
              
            }
            catch
            {

            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {if(Input.IsEnabled)
            {     
                try
                {
                    
                        FileSavePicker picker = new FileSavePicker();
                        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                        picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                        picker.DefaultFileExtension = ".txt";
                        picker.SuggestedFileName = "Source";
                        StorageFile file = await picker.PickSaveFileAsync();
                        if (file != null)
                        {
                            CachedFileManager.DeferUpdates(file);
                            await FileIO.WriteLinesAsync(file, Input.Text.Split('\r'));


                            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                        }
                    
                    else
                    {

                    }
                }
                catch
                {

                }
            }
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            if (Input.IsEnabled)
            {
                List<string> lista;
                Inline2018.Inline18 test = new Inline2018.Inline18(false);
                lista=await test.Run(Input.Text.Split('\r'));
                Output.Text = string.Join(Environment.NewLine, lista.ToArray());
            }

        }
        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Cache the cursor set before pointer enter on button.
            cursorBeforePointerEntered = Window.Current.CoreWindow.PointerCursor;
            // Set button cursor.
            Window.Current.CoreWindow.PointerCursor = buttonCursor;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Change the cursor back.
            Window.Current.CoreWindow.PointerCursor = cursorBeforePointerEntered;
        }

     


        private void Show_output_Click(object sender, RoutedEventArgs e)
        {
            OUTPUT_PANE.IsPaneOpen = !OUTPUT_PANE.IsPaneOpen;
        }

        private void Hide_output_Click(object sender, RoutedEventArgs e)
        {

            OUTPUT_PANE.IsPaneOpen = !OUTPUT_PANE.IsPaneOpen;

        }

        private async void Include_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".txt");
                var files = await picker.PickMultipleFilesAsync();
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                
                foreach (var file in files)
                {
                    string content = await FileIO.ReadTextAsync(file);
                    var newF = await storageFolder.CreateFileAsync(file.Name);
                    
                    await FileIO.WriteTextAsync(newF, content);
                }
            }
            catch { }
        }

        private async void Export_click(object sender, RoutedEventArgs e)
        {
            if (Input.IsEnabled)
            {
                try
                {
                    FileSavePicker picker = new FileSavePicker();
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                    picker.DefaultFileExtension = ".txt";
                    picker.SuggestedFileName = "Document";
                    StorageFile file = await picker.PickSaveFileAsync();
                    if (file != null)
                    {
                        CachedFileManager.DeferUpdates(file);
                        await FileIO.WriteLinesAsync(file, Output.Text.Split('\r'));


                        FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    }
                }
                catch
                {

                }
            }
        }
        string tekst="ˇ";
        bool liveRun = false;

        public object MyEvent { get; private set; }

        private  void Live_Click(object sender, RoutedEventArgs e)
        {

           liveRun=!liveRun;
            if (liveRun)
            {
                Live.Background = new SolidColorBrush(Windows.UI.Colors.Silver);
                Live.Icon = new SymbolIcon(Symbol.Switch);
            }
            else
            { Live.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                Live.Icon = new SymbolIcon(Symbol.Sort);
            }
        }

        
       
        private async void Input_KeyUp(object sender, KeyRoutedEventArgs e)
        { 
            if (liveRun && e.Key==VirtualKey.Enter&&Input.Text!=string.Empty)
            {
                
                List<string> lista;
                Inline2018.Inline18 test = new Inline2018.Inline18(true);

                lista = await test.Run(Input.Text.Replace(tekst, "").Split('\r'));
                tekst = Input.Text;
                Output.Text += string.Join(Environment.NewLine, lista.ToArray());
            }
        }

        
    }
}
