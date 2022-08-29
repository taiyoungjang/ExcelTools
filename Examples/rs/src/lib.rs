#[allow(dead_code)]
pub fn read_string(reader: &mut binary_reader::BinaryReader) -> String {
    let len = reader.read_u32().unwrap() as usize;
    let vec = reader.read_bytes(len).unwrap();
    let ret = String::from_utf8(Vec::from(vec)).map_err(|err| {
        std::io::Error::new(
            std::io::ErrorKind::InvalidData,
            format!("failed to convert to string: {:?}", err),
        )
    }).unwrap();
    ret
}
