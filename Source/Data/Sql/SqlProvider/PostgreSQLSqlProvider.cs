﻿using System;

namespace BLToolkit.Data.Sql.SqlProvider
{
	using DataProvider;

	public class PostgreSQLSqlProvider : BasicSqlProvider
	{
		public PostgreSQLSqlProvider(DataProviderBase dataProvider) : base(dataProvider)
		{
		}

		public override ISqlExpression ConvertExpression(ISqlExpression expr)
		{
			if (expr is SqlBinaryExpression)
			{
				SqlBinaryExpression be = (SqlBinaryExpression)expr;

				switch (be.Operation[0])
				{
					case '^': return new SqlBinaryExpression(be.Expr1, "#", be.Expr2);
				}
			}
			else if (expr is SqlFunction)
			{
				SqlFunction func = (SqlFunction) expr;

				switch (func.Name)
				{
					case "IndexOf":
						if (func.Parameters.Length == 2)
							return new SqlBinaryExpression(
								new SqlExpression("POSITION({0} in {1})", Precedence.Primary, func.Parameters[1], func.Parameters[0]),
								"-",
								new SqlValue(1),
								Precedence.Subtraction);

						var n = new SqlBinaryExpression(func.Parameters[2], "+", new SqlValue(1), Precedence.Additive);

						return new SqlExpression("POSITION({0} in {1})", Precedence.Primary,
							func.Parameters[1],
							new SqlFunction("SUBSTRING",
								func.Parameters[0],
								n,
								new SqlBinaryExpression(new SqlFunction("CHARACTER_LENGTH", func.Parameters[0]), "-", n, Precedence.Subtraction)));
				}
			}

			return base.ConvertExpression(expr);
		}
	}
}
