// Decompiled with JetBrains decompiler
// Type: Workflow.Services.Abstract.IGoalMessageService
// Assembly: Workflow.Services, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E78A2202-7C00-46B7-A598-F5FFE063B189
// Assembly location: C:\Users\ramiz\Desktop\WorkflowBin4\Workflow.Services.dll

using PageLoading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Workflow.DAL.Models;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Abstract
{
  public interface IGoalMessageService
  {
    Task<VmGoalMessage> Get(ApplicationUser currentUser, int id);

    Task<IEnumerable<VmGoalMessage>> GetPage(
      ApplicationUser currentUser,
      int? goalId,
      PageOptions pageOptions);

    Task<IEnumerable<VmGoalMessage>> GetUnreadPage(
      ApplicationUser currentUser,
      int? goalId,
      PageOptions pageOptions);

    Task<IEnumerable<VmGoalMessage>> GetRange(
      ApplicationUser currentUser,
      IEnumerable<int> ids);

    Task<VmGoalMessage> Create(ApplicationUser currentUser, VmGoalMessage message);

    Task Update(ApplicationUser currentUser, VmGoalMessage message);

    Task MarkAsRead(ApplicationUser currentUser, IEnumerable<int> ids);

    Task<VmGoalMessage> Delete(ApplicationUser currentUser, int id);

    Task<IEnumerable<VmGoalMessage>> DeleteRange(
      ApplicationUser currentUser,
      IEnumerable<int> ids);

    Task<VmGoalMessage> Restore(ApplicationUser currentUser, int id);

    Task<IEnumerable<VmGoalMessage>> RestoreRange(
      ApplicationUser currentUser,
      IEnumerable<int> ids);

    Task<int> GetUnreadCount(ApplicationUser currentUser, int? goalId);
  }
}
