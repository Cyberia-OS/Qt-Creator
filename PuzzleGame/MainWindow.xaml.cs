using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PuzzleGame
{
    public partial class MainWindow : Window
    {
        // Размер одной ячейки в пикселях. Те же числа, что и в MainWindow.xaml
        // (там же передаются как ConverterParameter для начальной позиции).
        private const double CellWidth = 95;
        private const double CellHeight = 54;

        // Длительность анимации перемещения плитки. Чем меньше число,
        // тем быстрее "едет" плитка.
        private const double MoveAnimationSeconds = 0.2;

        private GameViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (GameViewModel)DataContext;

            // Подписываемся на каждую плитку, которая уже есть на поле.
            foreach (var tile in _viewModel.Tiles)
            {
                tile.PropertyChanged += Tile_PropertyChanged;
            }

            // Когда нажимают "Новая игра" или "Перемешать", старые плитки
            // удаляются и создаются новые. Подписываемся на это событие,
            // чтобы каждая новая плитка тоже была подписана на анимацию.
            _viewModel.Tiles.CollectionChanged += Tiles_CollectionChanged;
        }

        private void Tiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Tile tile in e.OldItems)
                {
                    tile.PropertyChanged -= Tile_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Tile tile in e.NewItems)
                {
                    tile.PropertyChanged += Tile_PropertyChanged;
                }
            }
        }

        // Срабатывает каждый раз, когда у какой-либо плитки меняется
        // свойство (Number, IsEmpty, Row или Column). Нас интересуют
        // только Row и Column — именно по ним запускаем анимацию.
        private void Tile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Tile.Row) && e.PropertyName != nameof(Tile.Column))
                return;

            var tile = (Tile)sender;

            // Находим визуальный элемент (ContentPresenter), который
            // ItemsControl создал для этой плитки, чтобы анимировать
            // именно его Canvas.Left / Canvas.Top.
            var container = TilesItemsControl.ItemContainerGenerator.ContainerFromItem(tile) as FrameworkElement;
            if (container == null)
                return;

            double targetLeft = tile.Column * CellWidth;
            double targetTop = tile.Row * CellHeight;

            var duration = new Duration(System.TimeSpan.FromSeconds(MoveAnimationSeconds));

            var animateLeft = new DoubleAnimation
            {
                To = targetLeft,
                Duration = duration
            };

            var animateTop = new DoubleAnimation
            {
                To = targetTop,
                Duration = duration
            };

            container.BeginAnimation(Canvas.LeftProperty, animateLeft);
            container.BeginAnimation(Canvas.TopProperty, animateTop);
        }
    }
}
