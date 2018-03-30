namespace Sitecore.Support.ExperienceEditor.WebEdit.Commands
{
  using Sitecore;
  using Sitecore.Data.Items;
  using Sitecore.ExperienceEditor.Utils;
  using Sitecore.Shell.Framework.Commands;
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
      foreach (Item item in ItemUtility.FilterSameItems(itemsToFilter))
      {
        if (item.Access.CanWrite() && (!item.Locking.IsLocked() || item.Locking.HasLock()))
        {
          WorkflowUtility.ExecuteWorkflowCommandIfAvailable(item, args.CommandItem, args.CommentFields);
        }
      }
    }
  }
}