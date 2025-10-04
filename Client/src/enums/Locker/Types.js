// export const LockerTypes = {
//     personal: 'personal',
//     temporary: 'temporary',
//     handOver: 'handOver',
//     parcel: 'parcel',
//     common: 'common',
// }


export const LockerTypes = [
    { background: '#4dd0e1', type: 'personal' },
    { background: '#f06292', type: 'common' },
    { background: '#81c784', type: 'handOver' },
    { background: '#ffeb3b', type: 'parcel' },
    { background: '#ffccbc', border: '2px solid #ff8a65', type: 'unspecified' }
];

export const getLockerOptions = () => {
    return Object.values(LockerTypes);
};