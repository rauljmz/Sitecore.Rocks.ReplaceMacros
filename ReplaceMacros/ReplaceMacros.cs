
//TODO find out how to push the server component
using System.Text;
using System.Windows;
using Sitecore.VisualStudio.Data.DataServices;
using Sitecore.VisualStudio.Sites;

namespace Sitecore.VisualStudio
{
    using System.Linq;
    using Commands;
    using ContentTrees;
    using ContentTrees.Items;
    using Data;

    /// <summary>Defines the class.</summary>    
    [Command(Submenu = "Tools")]
    public class ReplaceMacros : CommandBase
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ReplaceMacros"/> class.</summary>
        public ReplaceMacros()
        {
            this.Text = "Replace Standard Value Macros";
            this.Group = "Tools";
            this.SortingValue = 5100;
        }

        #endregion

        #region Public Methods

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public override bool CanExecute(object parameter)
        {
            var context = parameter as ContentTreeContext;
            if (context == null)
            {
                return false;
            }

            if (!context.SelectedItems.Any(i=>i is ItemTreeViewItem))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            var context = parameter as ContentTreeContext;
            if (context == null)
            {
                return;
            }

            if (!context.SelectedItems.Any(i => i is ItemTreeViewItem))
            {
                return;
            }

            var executeCompleted = (ExecuteCompleted)(delegate(string response, ExecuteResult result)
                                                           {
                                                               if (!DataService.HandleExecute(response, result))
                                                               {
                                                                   return;
                                                               }
                                                               //TODO refresh existing open editors, response contains stringlist of ids of modified items
                                                               
                                                           });




            var stringBuilder = new StringBuilder();

            Site site = null;

            foreach (ItemTreeViewItem element in context.SelectedItems.OfType<ItemTreeViewItem>())
            {                
                if(site == null)
                {
                    site = element.Site;
                }
                else
                {
                    stringBuilder.Append("|");
                }

                if(site == element.Site)
                    stringBuilder.Append(string.Format("{0}:{1}",element.ItemUri.DatabaseName.Name,element.ItemUri.ItemId));
            }

            if (site != null)
                site.DataService.ExecuteAsync("Sitecore.Rocks.Server.Requests.ReplaceMacros,Sitecore.Rocks.Server.ReplaceMacros", executeCompleted,
                                              new[]
                                                  {                                                     
                                                      (object) stringBuilder.ToString()
                                                  }
                    );
        }

        #endregion
    }
}
