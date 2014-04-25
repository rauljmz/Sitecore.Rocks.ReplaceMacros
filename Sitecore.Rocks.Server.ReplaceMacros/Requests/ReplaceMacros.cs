using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;

namespace Sitecore.Rocks.Server.Requests
{
  using System.IO;
  using System.Xml;

  public class ReplaceMacros
  {
    public string Execute(string itemLocators)
    {        
        Assert.ArgumentNotNullOrEmpty(itemLocators, "items");

        var masterVariablesReplacer = Configuration.Factory.GetMasterVariablesReplacer();

        var result = new ListString('|');

        foreach (var itemlocator in itemLocators.Split('|'))
        {
            var separator = itemlocator.IndexOf(':');
            var databaseName = itemlocator.Substring(0, separator);

            var database = Configuration.Factory.GetDatabase(databaseName);
            if (database == null) continue;            

            var item = database.GetItem(itemlocator.Substring(separator+1));
            if (item != null)
            {
                item.Fields.ReadAll();
                var revision = item[FieldIDs.Revision];
                using (new EditContext(item))
                {
                    masterVariablesReplacer.ReplaceItem(item);
                }
                if(item[FieldIDs.Revision] != revision)
                {
                    result.Add(string.Format("{0}:{1}",databaseName,item.ID));
                }
            }
        }
        return result.ToString();
    }
  }
}