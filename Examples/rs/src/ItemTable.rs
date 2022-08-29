use std::collections::HashMap;
#[derive(Clone)]
#[allow(dead_code)]
#[allow(non_snake_case)]
pub struct Item
{
  pub Item_ID: i32,
  pub Name: String,
  pub Item_grade: i32,
  pub Require_lv: i32,
  pub Enchant_lv: i32,
  pub PhysicalAttack: i32,
  pub PhysicalDefense: i32,
  pub MagicalAttack: i32,
  pub MagicalDefense: i32,
  pub Critical: f32,
  pub HP: i32,
  pub KnockBackResist: i32,
  pub DictionaryType: i32,
  pub ItemType: i32,
  pub Gear_Score: i16,
  pub InventoryType: i16,
  pub UsageType: bool,
  pub Socket_quantity: i16,
  pub Removal_cost: i32,
  pub Belonging: i16,
  pub Sub_stats_quantity: i16,
  pub Stack: i32,
  pub DesignScroll_ID: i32,
  pub BindingSkill_ID: i32,
  pub BindingAttack_ID: i32,
  pub Manufacture_gold: i32,
  pub Manufacture_cash: i32,
  pub SummonCompanion_ID: i32,
  pub Next_itemID: i32,
  pub Next_item_price: i32,
  pub Next_Item_material: Vec<i32>,
  /// 젬의 경우 전체 필요한 수량을 여기에 적는다상위 젬을 만들기 위해 하위 젬이 4개 필요한 경우 4라고 기재
  pub Next_Item_material_quantity: Vec<i32>,
  pub Resource_Path: String,
  pub WeaponName: String,
  pub WeaponIndex: i16,
  pub PartName: Vec<String>,
  pub PartIndex: Vec<i16>,
  pub Icon_path: String,
  pub EXP: i32,
  pub Buy_cost: i32,
  pub Sell_reward: i32,
  pub Consignment_maxprice: i32,
  pub QuestBringer: i32,
  pub ItemEvent_ID: i32,
  pub Description: String,
  /// NETEASE-SH:방패등 서브아이템 아이디
  pub Sub_Item: i32,
  /// wlfh qkr:0: 맨손1: 왼손무기2: 오른손 무기
  pub WeaponType: i32,
  pub RandomBoxGroup_NO: Vec<i32>,
}
#[derive(Clone)]
#[allow(dead_code)]
#[allow(non_snake_case)]
pub struct ItemEffect
{
  pub Index: i32,
  pub Item_ID: i32,
  pub Effect_type: i32,
  pub Effect_min: f32,
  pub Effect_max: f32,
  pub Time_type: i32,
  pub Time_rate: f32,
  pub Time: f32,
  pub Duration: f32,
  pub Description: String,
}
#[derive(Clone)]
#[allow(dead_code)]
#[allow(non_snake_case)]
pub struct ItemEnchant
{
  pub Index: i32,
  pub Item_ID: i32,
  pub Enchant_lv: i32,
  pub Physical_attack: i32,
  pub Physical_defense: i32,
  pub Magic_attack: i32,
  pub Magic_defense: i32,
  pub Critical: f32,
  pub HP: i32,
  pub KnockBack_resist: i32,
  pub Material_IDS: Vec<i32>,
  pub Material_quantitys: Vec<i32>,
  pub Require_gold: i32,
  pub Require_cash: i32,
}
#[derive(Clone)]
#[allow(dead_code)]
#[allow(non_snake_case)]
pub struct ItemManufacture
{
  pub Index: i32,
  pub Subject_item_ID: i32,
  pub Material_item_ID: i32,
  pub Material_quantity: i32,
}
#[derive(Clone)]
#[allow(dead_code)]
#[allow(non_snake_case)]
pub struct RandomBoxGroup
{
  pub ID: i32,
  pub RandomItemGroup_NO: i32,
  pub ClassType: i32,
  pub Item_ID: i32,
  pub RatioAmount: i32,
  pub Item_Quantity: i32,
}
#[allow(dead_code)]
#[allow(non_snake_case)]
pub fn readStream(reader: &mut binary_reader::BinaryReader) {
  let (_Item_map, _Item_vec) = Item::readStream(reader);
  let (_ItemEffect_map, _ItemEffect_vec) = ItemEffect::readStream(reader);
  let (_ItemEnchant_map, _ItemEnchant_vec) = ItemEnchant::readStream(reader);
  let (_ItemManufacture_map, _ItemManufacture_vec) = ItemManufacture::readStream(reader);
  let (_RandomBoxGroup_map, _RandomBoxGroup_vec) = RandomBoxGroup::readStream(reader);
}
impl Item
{
  #[allow(dead_code)]
  pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<Item>,HashMap<i32,Item>) {
    let map:HashMap<i32,Item> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {
      let v = Item {
        Item_ID: reader.read_i32().unwrap(),
        Name: reader.read_cstr().unwrap(),
        Item_grade: reader.read_i32().unwrap(),
        Require_lv: reader.read_i32().unwrap(),
        Enchant_lv: reader.read_i32().unwrap(),
        PhysicalAttack: reader.read_i32().unwrap(),
        PhysicalDefense: reader.read_i32().unwrap(),
        MagicalAttack: reader.read_i32().unwrap(),
        MagicalDefense: reader.read_i32().unwrap(),
        Critical: reader.read_f32().unwrap(),
        HP: reader.read_i32().unwrap(),
        KnockBackResist: reader.read_i32().unwrap(),
        DictionaryType: reader.read_i32().unwrap(),
        ItemType: reader.read_i32().unwrap(),
        Gear_Score: reader.read_i16().unwrap(),
        InventoryType: reader.read_i16().unwrap(),
        UsageType: reader.read_bool().unwrap(),
        Socket_quantity: reader.read_i16().unwrap(),
        Removal_cost: reader.read_i32().unwrap(),
        Belonging: reader.read_i16().unwrap(),
        Sub_stats_quantity: reader.read_i16().unwrap(),
        Stack: reader.read_i32().unwrap(),
        DesignScroll_ID: reader.read_i32().unwrap(),
        BindingSkill_ID: reader.read_i32().unwrap(),
        BindingAttack_ID: reader.read_i32().unwrap(),
        Manufacture_gold: reader.read_i32().unwrap(),
        Manufacture_cash: reader.read_i32().unwrap(),
        SummonCompanion_ID: reader.read_i32().unwrap(),
        Next_itemID: reader.read_i32().unwrap(),
        Next_item_price: reader.read_i32().unwrap(),
        Next_Item_material: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i32().unwrap()).collect(),
        Next_Item_material_quantity: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i32().unwrap()).collect(),
        Resource_Path: reader.read_cstr().unwrap(),
        WeaponName: reader.read_cstr().unwrap(),
        WeaponIndex: reader.read_i16().unwrap(),
        PartName: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_cstr().unwrap()).collect(),
        PartIndex: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i16().unwrap()).collect(),
        Icon_path: reader.read_cstr().unwrap(),
        EXP: reader.read_i32().unwrap(),
        Buy_cost: reader.read_i32().unwrap(),
        Sell_reward: reader.read_i32().unwrap(),
        Consignment_maxprice: reader.read_i32().unwrap(),
        QuestBringer: reader.read_i32().unwrap(),
        ItemEvent_ID: reader.read_i32().unwrap(),
        Description: reader.read_cstr().unwrap(),
        Sub_Item: reader.read_i32().unwrap(),
        WeaponType: reader.read_i32().unwrap(),
        RandomBoxGroup_NO: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i32().unwrap()).collect(),
    }; (v.Item_ID,v) }).collect();
    (map.values().cloned().collect(),map)
  }
}
impl ItemEffect
{
  #[allow(dead_code)]
  pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<ItemEffect>,HashMap<i32,ItemEffect>) {
    let map:HashMap<i32,ItemEffect> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {
      let v = ItemEffect {
        Index: reader.read_i32().unwrap(),
        Item_ID: reader.read_i32().unwrap(),
        Effect_type: reader.read_i32().unwrap(),
        Effect_min: reader.read_f32().unwrap(),
        Effect_max: reader.read_f32().unwrap(),
        Time_type: reader.read_i32().unwrap(),
        Time_rate: reader.read_f32().unwrap(),
        Time: reader.read_f32().unwrap(),
        Duration: reader.read_f32().unwrap(),
        Description: reader.read_cstr().unwrap(),
    }; (v.Index,v) }).collect();
    (map.values().cloned().collect(),map)
  }
}
impl ItemEnchant
{
  #[allow(dead_code)]
  pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<ItemEnchant>,HashMap<i32,ItemEnchant>) {
    let map:HashMap<i32,ItemEnchant> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {
      let v = ItemEnchant {
        Index: reader.read_i32().unwrap(),
        Item_ID: reader.read_i32().unwrap(),
        Enchant_lv: reader.read_i32().unwrap(),
        Physical_attack: reader.read_i32().unwrap(),
        Physical_defense: reader.read_i32().unwrap(),
        Magic_attack: reader.read_i32().unwrap(),
        Magic_defense: reader.read_i32().unwrap(),
        Critical: reader.read_f32().unwrap(),
        HP: reader.read_i32().unwrap(),
        KnockBack_resist: reader.read_i32().unwrap(),
        Material_IDS: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i32().unwrap()).collect(),
        Material_quantitys: std::iter::repeat(reader.read_i32().unwrap()).map(|_|reader.read_i32().unwrap()).collect(),
        Require_gold: reader.read_i32().unwrap(),
        Require_cash: reader.read_i32().unwrap(),
    }; (v.Index,v) }).collect();
    (map.values().cloned().collect(),map)
  }
}
impl ItemManufacture
{
  #[allow(dead_code)]
  pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<ItemManufacture>,HashMap<i32,ItemManufacture>) {
    let map:HashMap<i32,ItemManufacture> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {
      let v = ItemManufacture {
        Index: reader.read_i32().unwrap(),
        Subject_item_ID: reader.read_i32().unwrap(),
        Material_item_ID: reader.read_i32().unwrap(),
        Material_quantity: reader.read_i32().unwrap(),
    }; (v.Index,v) }).collect();
    (map.values().cloned().collect(),map)
  }
}
impl RandomBoxGroup
{
  #[allow(dead_code)]
  pub fn readStream(reader: &mut binary_reader::BinaryReader) -> (Vec<RandomBoxGroup>,HashMap<i32,RandomBoxGroup>) {
    let map:HashMap<i32,RandomBoxGroup> = std::iter::repeat(reader.read_i32().unwrap()).map(|_| {
      let v = RandomBoxGroup {
        ID: reader.read_i32().unwrap(),
        RandomItemGroup_NO: reader.read_i32().unwrap(),
        ClassType: reader.read_i32().unwrap(),
        Item_ID: reader.read_i32().unwrap(),
        RatioAmount: reader.read_i32().unwrap(),
        Item_Quantity: reader.read_i32().unwrap(),
    }; (v.ID,v) }).collect();
    (map.values().cloned().collect(),map)
  }
}
