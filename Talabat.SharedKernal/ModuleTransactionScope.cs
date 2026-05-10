using System.Transactions;

namespace Talabat.SharedKernal;

public static class ModuleTransactionScope
{
	public static TransactionScope Create(
		IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
	{
		return new TransactionScope(
			TransactionScopeOption.Required,
			new TransactionOptions { IsolationLevel = isolationLevel },
			TransactionScopeAsyncFlowOption.Enabled);
	}
}
