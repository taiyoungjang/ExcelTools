#[allow(non_snake_case)]
mod ItemTable;

fn main() {
    println!("Hello, world!");
    let _ = ItemTable::RandomBoxGroup{
        ID: 1,
        RandomItemGroup_NO: 0,
        ClassType: 0,
        Item_ID: 0,
        RatioAmount: 0,
        Item_Quantity: 1,
    };
}
