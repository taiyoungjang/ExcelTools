use std::fs::File;
use binary_reader::{BinaryReader, Endian};

#[allow(non_snake_case)]
mod ItemTable;
mod lib;

fn main() {
    let mut file = File::open(r"..\Bytes\English\ItemTable.bytes").unwrap();
    let mut reader = BinaryReader::from_file(&mut file);
    reader.set_endian(Endian::Little);
    let _static_data = ItemTable::readStream(&mut reader);
    println!("{:?}", _static_data.ItemEffect_vec);
}
