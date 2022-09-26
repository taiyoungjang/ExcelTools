use std::io::prelude::*;
use std::collections::HashMap;
use std::io::BufReader;
use binary_reader::{BinaryReader, Endian};
  #[derive(Clone)]
  #[derive(Debug)]
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
  #[derive(Debug)]
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
  #[derive(Debug)]
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
  #[derive(Debug)]
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
  #[derive(Debug)]
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
  pub fn readStream(reader: &mut BinaryReader) -> StaticData {
    let _streamLength = reader.length;
    let _hashLength = reader.read_i8().unwrap() as usize;
    let _hashBytes = reader.read(_hashLength);
    let _decompressedSize = reader.read_u32().unwrap() as usize;
    let mut _decompressed = Vec::new();
    let mut _compressedSize = reader.read_u32().unwrap() as usize;
    let _bytes = BufReader::with_capacity(8192, reader.read(_streamLength-1-_hashLength-4-4).unwrap().as_ref());
    // md5 compute hash
    let mut decompressor = bzip2::read::MultiBzDecoder::new(_bytes);
    decompressor.read_to_end(&mut _decompressed).unwrap();
    let mut decompressReader = binary_reader::BinaryReader::from_vec(&mut _decompressed);
    decompressReader.set_endian(Endian::Little);
    let (Item_vec, Item_map) = Item::readStream(&mut decompressReader);
    let (ItemEffect_vec, ItemEffect_map) = ItemEffect::readStream(&mut decompressReader);
    let (ItemEnchant_vec, ItemEnchant_map) = ItemEnchant::readStream(&mut decompressReader);
    let (ItemManufacture_vec, ItemManufacture_map) = ItemManufacture::readStream(&mut decompressReader);
    let (RandomBoxGroup_vec, RandomBoxGroup_map) = RandomBoxGroup::readStream(&mut decompressReader);
    StaticData{
      Item_vec, Item_map,
      ItemEffect_vec, ItemEffect_map,
      ItemEnchant_vec, ItemEnchant_map,
      ItemManufacture_vec, ItemManufacture_map,
      RandomBoxGroup_vec, RandomBoxGroup_map,
    }
  }
  impl Item
  {
    #[allow(dead_code)]
    pub fn readStream(reader: &mut BinaryReader) -> (Vec<Item>,HashMap<i32,Item>) {
      let size = reader.read_u32().unwrap() as usize;
      let mut vec: Vec<Item> = Vec::with_capacity(size);
      let mut map:HashMap<i32,Item> = HashMap::with_capacity(size);
      for _ in 0..size {
        let v = Item {
          Item_ID: reader.read_i32().unwrap(),
          Name: crate::lib::read_string(reader),
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
          Next_Item_material: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i32().unwrap()).take(size).collect()},
          Next_Item_material_quantity: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i32().unwrap()).take(size).collect()},
          Resource_Path: crate::lib::read_string(reader),
          WeaponName: crate::lib::read_string(reader),
          WeaponIndex: reader.read_i16().unwrap(),
          PartName: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||crate::lib::read_string(reader)).take(size).collect()},
          PartIndex: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i16().unwrap()).take(size).collect()},
          Icon_path: crate::lib::read_string(reader),
          EXP: reader.read_i32().unwrap(),
          Buy_cost: reader.read_i32().unwrap(),
          Sell_reward: reader.read_i32().unwrap(),
          Consignment_maxprice: reader.read_i32().unwrap(),
          QuestBringer: reader.read_i32().unwrap(),
          ItemEvent_ID: reader.read_i32().unwrap(),
          Description: crate::lib::read_string(reader),
          Sub_Item: reader.read_i32().unwrap(),
          WeaponType: reader.read_i32().unwrap(),
          RandomBoxGroup_NO: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i32().unwrap()).take(size).collect()},
        };
        map.insert(v.Item_ID,v.clone());
        vec.push(v);
      }
      (vec,map)
    }
  }
  impl ItemEffect
  {
    #[allow(dead_code)]
    pub fn readStream(reader: &mut BinaryReader) -> (Vec<ItemEffect>,HashMap<i32,ItemEffect>) {
      let size = reader.read_u32().unwrap() as usize;
      let mut vec: Vec<ItemEffect> = Vec::with_capacity(size);
      let mut map:HashMap<i32,ItemEffect> = HashMap::with_capacity(size);
      for _ in 0..size {
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
          Description: crate::lib::read_string(reader),
        };
        map.insert(v.Index,v.clone());
        vec.push(v);
      }
      (vec,map)
    }
  }
  impl ItemEnchant
  {
    #[allow(dead_code)]
    pub fn readStream(reader: &mut BinaryReader) -> (Vec<ItemEnchant>,HashMap<i32,ItemEnchant>) {
      let size = reader.read_u32().unwrap() as usize;
      let mut vec: Vec<ItemEnchant> = Vec::with_capacity(size);
      let mut map:HashMap<i32,ItemEnchant> = HashMap::with_capacity(size);
      for _ in 0..size {
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
          Material_IDS: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i32().unwrap()).take(size).collect()},
          Material_quantitys: {let size = reader.read_i32().unwrap() as usize; std::iter::repeat_with(||reader.read_i32().unwrap()).take(size).collect()},
          Require_gold: reader.read_i32().unwrap(),
          Require_cash: reader.read_i32().unwrap(),
        };
        map.insert(v.Index,v.clone());
        vec.push(v);
      }
      (vec,map)
    }
  }
  impl ItemManufacture
  {
    #[allow(dead_code)]
    pub fn readStream(reader: &mut BinaryReader) -> (Vec<ItemManufacture>,HashMap<i32,ItemManufacture>) {
      let size = reader.read_u32().unwrap() as usize;
      let mut vec: Vec<ItemManufacture> = Vec::with_capacity(size);
      let mut map:HashMap<i32,ItemManufacture> = HashMap::with_capacity(size);
      for _ in 0..size {
        let v = ItemManufacture {
          Index: reader.read_i32().unwrap(),
          Subject_item_ID: reader.read_i32().unwrap(),
          Material_item_ID: reader.read_i32().unwrap(),
          Material_quantity: reader.read_i32().unwrap(),
        };
        map.insert(v.Index,v.clone());
        vec.push(v);
      }
      (vec,map)
    }
  }
  impl RandomBoxGroup
  {
    #[allow(dead_code)]
    pub fn readStream(reader: &mut BinaryReader) -> (Vec<RandomBoxGroup>,HashMap<i32,RandomBoxGroup>) {
      let size = reader.read_u32().unwrap() as usize;
      let mut vec: Vec<RandomBoxGroup> = Vec::with_capacity(size);
      let mut map:HashMap<i32,RandomBoxGroup> = HashMap::with_capacity(size);
      for _ in 0..size {
        let v = RandomBoxGroup {
          ID: reader.read_i32().unwrap(),
          RandomItemGroup_NO: reader.read_i32().unwrap(),
          ClassType: reader.read_i32().unwrap(),
          Item_ID: reader.read_i32().unwrap(),
          RatioAmount: reader.read_i32().unwrap(),
          Item_Quantity: reader.read_i32().unwrap(),
        };
        map.insert(v.ID,v.clone());
        vec.push(v);
      }
      (vec,map)
    }
  }
  #[allow(dead_code)]
  #[allow(non_snake_case)]
  pub struct StaticData {
    pub Item_vec: Vec<Item>,
    pub Item_map: HashMap<i32,Item>,
    pub ItemEffect_vec: Vec<ItemEffect>,
    pub ItemEffect_map: HashMap<i32,ItemEffect>,
    pub ItemEnchant_vec: Vec<ItemEnchant>,
    pub ItemEnchant_map: HashMap<i32,ItemEnchant>,
    pub ItemManufacture_vec: Vec<ItemManufacture>,
    pub ItemManufacture_map: HashMap<i32,ItemManufacture>,
    pub RandomBoxGroup_vec: Vec<RandomBoxGroup>,
    pub RandomBoxGroup_map: HashMap<i32,RandomBoxGroup>,
  }
