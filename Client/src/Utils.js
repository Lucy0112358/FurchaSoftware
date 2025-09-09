export function fromCamelCase(str) {
  return str.replace(/([a-z])([A-Z])/g, "$1 $2");
}

export function fromCamelCasePretty(str) {
  return str
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/^./, s => s.toUpperCase());
}
