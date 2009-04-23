﻿using System;
using System.Text;

namespace BLToolkit.Data.Sql.SqlProvider
{
	public interface ISqlProvider
	{
		StringBuilder  BuildSql         (SqlBuilder sqlBuilder, StringBuilder sb, int indent);
		ISqlExpression ConvertExpression(ISqlExpression expression);
		ISqlPredicate  ConvertPredicate (ISqlPredicate  predicate);
	}
}
