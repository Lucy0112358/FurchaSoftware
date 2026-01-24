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

export const getLockerColor = (lockerType) => {
  switch (lockerType.toLowerCase()) {
    case 'personal':
      return '#45d8f9';
    case 'temporary':
      return '#F97BA2';
    case 'common':
      return '#ff7da8';
    case 'handover':
      return '#97d381';
    case 'parcel':
      return '#ffe680';
    default:
      return '#dbdbdb';
  }
}

