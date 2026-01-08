export function getIndexConfig(Charset) {
  return {
    tokenize: "forward",
    encoder: Charset.LatinBalance,
    document: {
      id: "id",
      index: [
        { field: "title", tokenize: "forward", resolution: 9 },
        { field: "body", tokenize: "forward", resolution: 5, minlength: 2 }
      ]
    }
  };
}
