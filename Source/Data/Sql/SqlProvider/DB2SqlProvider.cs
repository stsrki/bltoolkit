﻿using System;
using System.Text;

namespace BLToolkit.Data.Sql.SqlProvider
{
	using DataProvider;

	public class DB2SqlProvider : BasicSqlProvider
	{
		public DB2SqlProvider(DataProviderBase dataProvider) : base(dataProvider)
		{
		}

		protected override void BuildSelectClause(StringBuilder sb)
		{
			if (SqlBuilder.From.Tables.Count == 0)
			{
				AppendIndent(sb);
				sb.Append("SELECT").AppendLine();
				BuildColumns(sb);
				AppendIndent(sb);
				sb.Append("FROM SYSIBM.SYSDUMMY1 FETCH FIRST 1 ROW ONLY").AppendLine();
			}
			else
				base.BuildSelectClause(sb);
		}

		public override ISqlExpression ConvertExpression(ISqlExpression expr)
		{
			if (expr is SqlBinaryExpression)
			{
				SqlBinaryExpression be = (SqlBinaryExpression)expr;

				switch (be.Operation[0])
				{
					case '%': return new SqlFunction("MOD",    be.Expr1, be.Expr2);
					case '&': return new SqlFunction("BITAND", be.Expr1, be.Expr2);
					case '|': return new SqlFunction("BITOR",  be.Expr1, be.Expr2);
					case '^': return new SqlFunction("BITXOR", be.Expr1, be.Expr2);
				}
			}
			else if (expr is SqlFunction)
			{
				SqlFunction func = (SqlFunction) expr;

				switch (func.Name)
				{
					case "CHARACTER_LENGTH": return new SqlFunction("LENGTH", func.Parameters);
					case "IndexOf":
						return new SqlBinaryExpression(
							func.Parameters.Length == 2?
								new SqlFunction("LOCATE", func.Parameters[1], func.Parameters[0]):
								new SqlFunction("LOCATE",
									func.Parameters[1],
									func.Parameters[0],
									new SqlBinaryExpression(func.Parameters[2], "+", new SqlValue(1), Precedence.Additive)),
							"-",
							new SqlValue(1),
							Precedence.Subtraction);
		}
			}

			return base.ConvertExpression(expr);
		}
	}
}
