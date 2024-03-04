using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using CrossesZeros;
using Windows.Services.Store;
using Windows.UI.Composition;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CrossesZerosXAML
{
    public struct CellTag
    {
        public int x;
        public int y;
        public CellTag(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public sealed partial class MainPage : Page
    {
        private Game game = null;
        private double gameGridSize = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GamePage_Loaded(object sender, RoutedEventArgs e)
        {
            gameGridSize = Math.Min(MainGrid.ActualWidth, MainGrid.ActualHeight) * 0.75;
            GameGrid.Width = gameGridSize;
            GameGrid.Height = gameGridSize;
            GameBorder.Height = gameGridSize;
            GameBorder.Width = gameGridSize;
        }

        private void Rect_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var rect = sender as Windows.UI.Xaml.Shapes.Rectangle;
            var tag = (CellTag)rect.Tag;
            
            if (game.Player2 is ConsolePlayer)
                game.MakeGameStep(tag.x, tag.y);
            else
            {
                game.MakeGameStep(tag.x, tag.y);
                if(!game.GameCompleted)
                    game.MakeGameStep(-1, -1);
            }

            UpdateGameField(game.Field);
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            BeginNewGame();
        }

        private async void Game_GameCompletedEvent(object sender)
        {
            string winner = "";
            if (game.Winner != Role.None)
                winner = $"The winner is - {game.Winner}!";
            else
                winner = "Draw!";
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Game over! " + winner;
            dialog.PrimaryButtonText = "Restart";
            dialog.CloseButtonText = "Ok...";
            dialog.DefaultButton = ContentDialogButton.Primary;
            //dialog.Content = new ContentDialogContent();

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
                BeginNewGame();
        }

        private void UpdateGameField(GameField field)
        {
            for(int i = 0; i < game.Field.FieldSize; i++)
                for(int j = 0; j < game.Field.FieldSize; j++)
                {
                    FontIcon icon = new FontIcon()
                    {
                        FontFamily = new FontFamily("Segoe MDL2 Assets"),
                        FontSize = gameGridSize / field.FieldSize / 2,
                        Foreground = new SolidColorBrush(Colors.Black)
                    };

                    switch (field.Cells[i, j].CellRole)
                    {
                        case Role.Crosses:
                            icon.Glyph = "\uE947";
                            break;
                        case Role.Zeros:
                            icon.Glyph = "\uEA3A";
                            break;
                        default:
                            break;

                    }              
                    Grid.SetColumn(icon, i);
                    Grid.SetRow(icon, j);
                    icon.HorizontalAlignment = HorizontalAlignment.Center;
                    icon.VerticalAlignment = VerticalAlignment.Center;
                    GameGrid.Children.Add(icon);
                }
            
        }

        private void BeginNewGame()
        {
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();
            GameGrid.Children.Clear();

            int FieldSize = (int)GameFieldSizeBox.Value;
            for (int i = 0; i < FieldSize; i++)
            {
                RowDefinition row = new RowDefinition();
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1.0, GridUnitType.Star);
                row.Height = new GridLength(1.0, GridUnitType.Star);
                GameGrid.RowDefinitions.Add(row);
                GameGrid.ColumnDefinitions.Add(column);
            }

            for (int i = 0; i < FieldSize; i++)
                for (int j = 0; j < FieldSize; j++)
                {
                    var size = gameGridSize / FieldSize;
                    Rectangle rect = new Rectangle();
                    rect.Fill = (AcrylicBrush)Resources["CustomAcrylicInAppLuminosity"];
                    rect.RadiusX = 10; rect.RadiusY = 10;
                    rect.PointerPressed += Rect_PointerPressed;
                    rect.Height = size * 0.9;
                    rect.Width = size * 0.9;
                    rect.HorizontalAlignment = HorizontalAlignment.Center;
                    rect.VerticalAlignment = VerticalAlignment.Center;
                    rect.Tag = new CellTag(i, j);
                    Grid.SetColumn(rect, i);
                    Grid.SetRow(rect, j);
                    GameGrid.Children.Add(rect);
                }

            ConsolePlayer player1 = new ConsolePlayer(Role.Crosses);
            Player player2 = null;

            switch (opponentComboBox.SelectedIndex)
            {
                case 0:
                    player2 = new ConsolePlayer(Role.Zeros);
                    break;
                case 1:
                    player2 = new StupidBotPlayer(Role.Zeros);
                    break;
                case 2:
                    player2 = new SmarterBotPlayer(Role.Zeros);
                    break;
                case 3:
                    player2 = new SmartBotPlayer(Role.Zeros);
                    break;
                default:
                    break;
            }
            game = new Game(FieldSize, player1, player2);
            game.GameCompletedEvent += Game_GameCompletedEvent;
        }
    }
}
