namespace Snebur.Persistence.Common.Resourses;

public static class PgSqlResources
{
    public static string GetNextSortOrderAscFunction
        => @"
            CREATE OR REPLACE FUNCTION get_next_sort_order_asc(table_name TEXT, column_name TEXT)
            RETURNS INTEGER AS $$
            DECLARE
                next_sort_order INTEGER;
                sql_query TEXT;
            BEGIN
                sql_query := format('SELECT COALESCE(MAX(%I) + 1, 1) FROM %I', column_name, table_name);
                EXECUTE sql_query INTO next_sort_order;
                RETURN next_sort_order;
            END;
            $$ LANGUAGE plpgsql;
        ";

    public static string GetNextSortOrderDescFunction
        => @"
            CREATE OR REPLACE FUNCTION get_next_sort_order_desc(table_name TEXT, column_name TEXT)
            RETURNS INTEGER AS $$
            DECLARE
                next_sort_order INTEGER;
                sql_query TEXT;
            BEGIN
                sql_query := format('SELECT COALESCE(MIN(%I) - 1, -1) FROM %I', column_name, table_name);
                EXECUTE sql_query INTO next_sort_order;
                RETURN next_sort_order;
            END;
            $$ LANGUAGE plpgsql;
        ";
}
