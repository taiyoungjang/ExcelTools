namespace TableGenerate;

public class Column
{
    // 한셀에 여러개의 데이터가 들어가는 경우
    public bool array_one_cell;
    public bool is_key;
    public int data_column_index;
    public string var_name;
    public BaseType base_type;
    public int array_index;
    public bool is_array;
    public bool is_last_array;
    public string array_group_name;
    public bool is_generated;
    public bool is_out_string;
    public string type_name;
    public BaseType primitive_type;
    public string desc;
    public System.Reflection.TypeInfo TypeInfo;
    public bool bit_flags => !string.IsNullOrEmpty(str_bit_flags);
    public string str_bit_flags;
    public string min_value;
    public string max_value;
    public string json;
    public bool blue_print_type = false;
};
