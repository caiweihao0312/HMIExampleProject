using Sale.BLL;
using Sale.Domain;
using System;
using System.Windows;

namespace Sale.AppDemo
{
    public partial class MainWindow : Window
    {
        private ProductService _service = new ProductService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            dgProducts.ItemsSource = _service.GetAll();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product { Name = "新商品", Count = 10 };
            _service.Add(product);
            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is Product product)
            {
                _service.Delete(product.Id);
                LoadData();
            }
        }

        private void btnCreateDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _service.CreateDatabaseIfNotExists();
                MessageBox.Show("本地数据库文件已创建。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建数据库失败：" + ex.Message);
            }
        }

        private void BtnLoadDb_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}