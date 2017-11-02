using AdminWpfApp.ManageTopicProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminWpfApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ManageTopicClient manageTopic;
		private MainWindowState state;
		public MainWindow()
		{
			InitializeComponent();
			manageTopic = new ManageTopicClient("BasicHttpBinding_IManageTopic");
			state = new MainWindowState();
			state.VisibleTopics = new ObservableCollection<Topic>(manageTopic.GetTopics());
			foreach(Topic t in state.VisibleTopics)
			{
				state.CurrentTopic = t.Id;
				break;
			}
			this.DataContext = state;
		}

		private void TopicAdd_Click(object sender, RoutedEventArgs e)
		{
			var id = manageTopic.CreateTopic(newTopicName.Text);
			state.UpdateVisibleTopics(manageTopic.GetTopics());
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{

		}

		private void RunReport_Click(object sender, RoutedEventArgs e)
		{

		}

		private void TopicDelete_Click(object sender, RoutedEventArgs e)
		{
			var id = manageTopic.DeleteTopic(state.CurrentTopic);
			state.UpdateVisibleTopics(manageTopic.GetTopics());
		}
	}

	public class MainWindowState
	{
		public MainWindowState()
		{
			System.Diagnostics.Trace.WriteLine("creating a new mainwindowstate state obj");
		}

		public ObservableCollection<Topic> VisibleTopics { get; set; }
		public int CurrentTopic { get; set; }

		public void UpdateVisibleTopics(IEnumerable<Topic> newTopicList)
		{
			List<int> foundIndexes = new List<int>();
			int originalLength = VisibleTopics.Count;

			// replace or add topics, keeping track of the ones you've touched
			foreach(Topic newTopic in newTopicList)
			{
				bool replacedExistingTopic = false;
				for(int i = 0; i < VisibleTopics.Count; i++)
				{
					if (newTopic.Id == VisibleTopics[i].Id)
					{
						VisibleTopics[i] = newTopic;
						foundIndexes.Add(i);
						replacedExistingTopic = true;
						break;
					}
				}
				if (!replacedExistingTopic)
				{
					VisibleTopics.Add(newTopic);
				}
			}

			// now delete no-longer-present topics
			List<int> indexesToRemove = new List<int>();
			for(int i = 0; i < originalLength; i++)
			{
				if(!foundIndexes.Contains(i))
				{
					indexesToRemove.Add(i);
				}
			}
			// delete in reverse order so we don't mes up the indexes
			if (indexesToRemove.Count > 0)
			{
				for (int i = indexesToRemove.Count-1; i >= 0; i--)
				{
					VisibleTopics.Remove(VisibleTopics[indexesToRemove[i]]);
				}
			}

		}
	}
}
