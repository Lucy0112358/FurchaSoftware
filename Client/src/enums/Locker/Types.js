export const LockerTypes = {
    personal: 'personal',
    temporary: 'temporary',
    handOver: 'handOver',
    parcel: 'parcel',
    common: 'common',
}

export const getLockerOptions = () => {
    return Object.values(LockerTypes);
};