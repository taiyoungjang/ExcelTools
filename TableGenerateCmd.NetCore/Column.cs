﻿namespace TableGenerate;

public class Column
{
    public bool is_key;
    public int data_column_index;
    public string var_name;
    public eBaseType base_type;
    public int array_index;
    public bool is_array;
    public bool is_last_array;
    public string array_group_name;
    public bool is_generated;
    public bool is_out_string;
    public string type_name;
    public eBaseType primitive_type;
    public string desc;
    public System.Reflection.TypeInfo TypeInfo;
    public bool bit_flags => !string.IsNullOrEmpty(str_bit_flags);
    public string str_bit_flags;
    public string min_value;
    public string max_value;
};