using CrawlerLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace WebCrawler
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private CancellationTokenSource _cancellationSource;
		private readonly Crawler _crawler;

		private readonly Regex _urlRegex = new Regex(@"^http(s)?:\/\/[a-z_0-9\/\-\.\?]+$", RegexOptions.IgnoreCase);
		private readonly Regex _levelRegex = new Regex(@"^[0-9]+$", RegexOptions.IgnoreCase);

		public MainWindow()
		{
			_crawler = new Crawler(
				new Parser(),
				new Searcher(),
				new Downloader()
			);
			InitializeComponent();
		}

		private bool ValidateInput()
		{
			return _urlRegex.IsMatch(TxtUrl.Text) && _levelRegex.IsMatch(TxtNestingLevel.Text) && !string.IsNullOrWhiteSpace(TxtPhrase.Text);
		}

		private void SwitchButtons(bool started)
		{
			BtnStart.IsEnabled = !started;
			BtnCancel.IsEnabled = started;
		}

		private async void BtnStart_Click(object sender, RoutedEventArgs e)
		{
			LblMessage.Content = string.Empty;
			if (!ValidateInput())
			{
				LblMessage.Content = "Source URL or Nesting level are invalid!";
				return;
			}

			SwitchButtons(true);

			if (_cancellationSource != null)
			{
				_cancellationSource.Dispose();
			}
			_cancellationSource = new CancellationTokenSource();
			

			string sourceUrl = TxtUrl.Text;
			string searchPhrase = TxtPhrase.Text;
			int nestingLevel = Convert.ToInt32(TxtNestingLevel.Text);

			_crawler.NestingLevel = nestingLevel;
			_crawler.SearchPhrase = searchPhrase;
			
			try
			{
				IEnumerable<string> urls = await _crawler.CrawlAsync(sourceUrl, _cancellationSource.Token);
				Dispatcher.Invoke(() =>
				{
					LbResults.ItemsSource = urls;
				});
			}
			catch (OperationCanceledException ex)
			{
				Debug.WriteLine(ex.Message);
			}

			SwitchButtons(false);
		}

		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			_cancellationSource.Cancel();
			SwitchButtons(false);
		}
	}
}
