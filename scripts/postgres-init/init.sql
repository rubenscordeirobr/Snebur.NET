 CREATE DATABASE identitydb
    WITH OWNER "snebur_user"
    ENCODING 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TEMPLATE template0;
    
\connect identitydb snebur_user

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

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