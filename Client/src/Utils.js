export function fromCamelCase(str) {
  return str.replace(/([a-z])([A-Z])/g, "$1 $2");
}

export function fromCamelCasePretty(str) {
  return str
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/^./, s => s.toUpperCase());
}

export const getNameStartLetter = (name) => {
  if (!name) return '';
  return name
    .split(' ')
    .map((n) => n.charAt(0).toUpperCase())
    .join('');
};
