namespace Sitecore.Support.ExperienceEditor.WebEdit.Commands
{
  using Sitecore;
  using Sitecore.Globalization;
  using Sitecore.Data.Items;
  using Sitecore.ExperienceEditor.Utils;
  using Sitecore.Layouts;
  using Sitecore.Workflows.Simple;
  using System;
  using System.Collections.Generic;

  [Serializable]
  public class WorkflowWithDatasourceItems : Sitecore.Shell.Framework.Commands.Workflow
  {
    [UsedImplicitly]
    protected void WorkflowCompleteCallback(WorkflowPipelineArgs args)
    {
      base.WorkflowCompleteCallback(args);
      List<Item> itemsToFilter = ItemUtility.GetItemsFromLayoutDefinedDatasources(args.DataItem, Context.Device, args.DataItem.Language);
      itemsToFilter.AddRange(ItemUtility.GetPersonalizationRulesItems(args.DataItem, Context.Device, args.DataItem.Language));
      itemsToFilter.AddRange(ItemUtility.GetTestItems(args.DataItem, Context.Device, args.DataItem.Language));
      #region Added code
      itemsToFilter.AddRange(this.GetPageDatasources(args.DataItem, Context.Device, args.DataItem.Language));
      #endregion
      foreach (Item item in ItemUtility.FilterSameItems(itemsToFilter))
      {
        if (item.Access.CanWrite() && (!item.Locking.IsLocked() || item.Locking.HasLock()))
        {
          WorkflowUtility.ExecuteWorkflowCommandIfAvailable(item, args.CommandItem, args.CommentFields);
        }
      }
    }
    #region Added code
    protected List<Item> GetPageDatasources(Item item, DeviceItem deviceItem, Language language = null)
    {
      List<Item> list = new List<Item>();
      foreach (RenderingReference reference in item.Visualization.GetRenderings(deviceItem, true))
      {
        Item ds = item.Database.GetItem(reference.Settings.DataSource, language ?? Language.Current);
        if (ds != null)
        {
          list.Add(ds);
        }
      }
      return list;
    }
    #endregion

  }
}