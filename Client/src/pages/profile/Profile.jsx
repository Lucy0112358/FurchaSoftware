import React, { useEffect, useState } from 'react';
import { getAuthUserData } from '../../redux/slice/authSlice';
import { useDispatch, useSelector } from 'react-redux';
import { changePassword, editAuthUser } from '../../redux/api/authApi';

const Profile = () => {
    const [isEditing, setIsEditing] = useState(false);
    const [isChangingPassword, setIsChangingPassword] = useState(false);
    const authuserData = useSelector(getAuthUserData);
    const dispatch = useDispatch();
    const [profile, setProfile] = useState({
        name: '',
        surname: '',
        email: '',
        phone: ''
    });

    const [passwordData, setPasswordData] = useState({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
    });

    const [errors, setErrors] = useState({
        profile: {},
        password: {}
    });

    // Сохраняем исходные данные для отката
    const [originalProfile, setOriginalProfile] = useState({
        name: '',
        surname: '',
        email: '',
        phone: ''
    });

    useEffect(() => {
        // Инициализация профиля из authuserData
        const userData = {
            name: authuserData.name || '',
            surname: authuserData.surname || '',
            email: authuserData.email || '',
            phone: authuserData.phone || ''
        };
        setProfile(userData);
        setOriginalProfile(userData); // Сохраняем исходные данные
    }, [authuserData]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProfile(prev => ({
            ...prev,
            [name]: value
        }));
        // Очищаем ошибку при изменении поля
        if (errors.profile[name]) {
            setErrors(prev => ({
                ...prev,
                profile: {
                    ...prev.profile,
                    [name]: ''
                }
            }));
        }
    };

    const handlePasswordChange = (e) => {
        const { name, value } = e.target;
        setPasswordData(prev => ({
            ...prev,
            [name]: value
        }));
        // Очищаем ошибку при изменении поля
        if (errors.password[name]) {
            setErrors(prev => ({
                ...prev,
                password: {
                    ...prev.password,
                    [name]: ''
                }
            }));
        }
    };

    const validateProfile = () => {
        const newErrors = {};
        
        if (!profile.name.trim()) {
            newErrors.name = 'Name is required';
        }
        
        if (!profile.surname.trim()) {
            newErrors.surname = 'Surname is required';
        }
        
        if (profile.phone && !/^\+?[\d\s\-()]+$/.test(profile.phone)) {
            newErrors.phone = 'Please enter a valid phone number';
        }

        setErrors(prev => ({
            ...prev,
            profile: newErrors
        }));

        return Object.keys(newErrors).length === 0;
    };

    const validatePassword = () => {
        const newErrors = {};
        
        if (!passwordData.currentPassword) {
            newErrors.currentPassword = 'Current password is required';
        }
        
        if (!passwordData.newPassword) {
            newErrors.newPassword = 'New password is required';
        } else if (passwordData.newPassword.length < 6) {
            newErrors.newPassword = 'Password must be at least 6 characters long';
        } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(passwordData.newPassword)) {
            newErrors.newPassword = 'Password must contain at least one uppercase letter, one lowercase letter and one number';
        }
        
        if (!passwordData.confirmPassword) {
            newErrors.confirmPassword = 'Please confirm your password';
        } else if (passwordData.newPassword !== passwordData.confirmPassword) {
            newErrors.confirmPassword = 'Passwords do not match';
        }

        setErrors(prev => ({
            ...prev,
            password: newErrors
        }));

        return Object.keys(newErrors).length === 0;
    };

    const handleSave = async () => {
        if (!validateProfile()) {
            return;
        }

        try {
            // Предполагаем, что editAuthUser возвращает Promise
            const result = await dispatch(editAuthUser(profile)).unwrap();
            
            // Если успешно - обновляем исходные данные и выходим из режима редактирования
            setOriginalProfile(profile);
            setIsEditing(false);
            
        } catch (error) {
            // Если ошибка - восстанавливаем исходные данные
            console.error('Error saving profile:', error);
            setProfile(originalProfile); // Восстанавливаем исходные данные
            // Можно показать сообщение об ошибке
            alert('Failed to save profile. Please try again.');
        }
    };

    const handleSavePassword = () => {
        if (!validatePassword()) {
            return;
        }
        // Здесь API запрос для смены пароля
        // Предполагаем асинхронный запрос
        somePasswordChangeAPI(passwordData)
            .then(() => {
                console.log('Password changed successfully');
                setIsChangingPassword(false);
                setPasswordData({
                    currentPassword: '',
                    newPassword: '',
                    confirmPassword: ''
                });
            })
            .catch(error => {
                console.error('Error changing password:', error);
                // В случае ошибки парольные поля остаются заполненными
                alert('Failed to change password. Please try again.');
            });
    };

    const handleCancel = () => {
        // Восстанавливаем исходные данные при отмене
        setProfile(originalProfile);
        setErrors(prev => ({ ...prev, profile: {} }));
        setIsEditing(false);
    };

    const handleCancelPassword = () => {
        setIsChangingPassword(false);
        setPasswordData({
            currentPassword: '',
            newPassword: '',
            confirmPassword: ''
        });
        setErrors(prev => ({ ...prev, password: {} }));
    };

    // Если ваш editAuthUser не возвращает Promise, можно использовать такой подход:
    const handleSaveAlternative = () => {
        if (!validateProfile()) {
            return;
        }

        // Сохраняем текущие изменения перед отправкой
        const currentChanges = { ...profile };
        
        dispatch(editAuthUser(profile))
            .then((response) => {
                // Проверяем успешность ответа
                if (response && response.success !== false) {
                    // Если успешно - обновляем исходные данные
                    setOriginalProfile(currentChanges);
                    setIsEditing(false);
                } else {
                    // Если ошибка в ответе - восстанавливаем данные
                    setProfile(originalProfile);
                    alert('Failed to save profile. Please try again.');
                }
            })
            .catch((error) => {
                // Если ошибка запроса - восстанавливаем данные
                console.error('Error saving profile:', error);
                setProfile(originalProfile);
                alert('Failed to save profile. Please try again.');
            });
    };

    // Заглушка для API смены пароля
    const somePasswordChangeAPI = (passwordData) => {
        return new Promise((resolve, reject) => {
           dispatch(changePassword(passwordData))
        });
    };

    return (
        <div className="min-h-screen px-4 sm:px-6 lg:px-8">
            <div className="max-w-2xl mx-auto">
                <div className="bg-white shadow rounded-lg">
                    {/* Заголовок */}
                    <div className="px-6 py-4 border-b border-gray-200">
                        <div className="flex justify-between items-center">
                            <h1 className="text-2xl font-bold text-gray-900">Profile</h1>
                            {!isEditing && !isChangingPassword && (
                                <div className="flex space-x-4">
                                    <button
                                        onClick={() => setIsEditing(true)}
                                        className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
                                    >
                                        Edit profile
                                    </button>
                                    <button
                                        onClick={() => setIsChangingPassword(true)}
                                        className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
                                    >
                                        Change password
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Контент профиля */}
                    <div className="px-6 py-6">
                        {/* Режим смены пароля (полностью заменяет основную форму) */}
                        {isChangingPassword ? (
                            <div className="space-y-6">
                                <h3 className="text-lg font-medium text-gray-900 mb-2">Change Password</h3>
                                
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Current password *
                                    </label>
                                    <input
                                        type="password"
                                        name="currentPassword"
                                        value={passwordData.currentPassword}
                                        onChange={handlePasswordChange}
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            errors.password.currentPassword 
                                                ? 'border-red-300 bg-red-50' 
                                                : 'border-gray-300'
                                        }`}
                                    />
                                    {errors.password.currentPassword && (
                                        <p className="text-red-600 text-sm mt-1">{errors.password.currentPassword}</p>
                                    )}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        New password *
                                    </label>
                                    <input
                                        type="password"
                                        name="newPassword"
                                        value={passwordData.newPassword}
                                        onChange={handlePasswordChange}
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            errors.password.newPassword 
                                                ? 'border-red-300 bg-red-50' 
                                                : 'border-gray-300'
                                        }`}
                                    />
                                    {errors.password.newPassword && (
                                        <p className="text-red-600 text-sm mt-1">{errors.password.newPassword}</p>
                                    )}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Confirm password *
                                    </label>
                                    <input
                                        type="password"
                                        name="confirmPassword"
                                        value={passwordData.confirmPassword}
                                        onChange={handlePasswordChange}
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            errors.password.confirmPassword 
                                                ? 'border-red-300 bg-red-50' 
                                                : 'border-gray-300'
                                        }`}
                                    />
                                    {errors.password.confirmPassword && (
                                        <p className="text-red-600 text-sm mt-1">{errors.password.confirmPassword}</p>
                                    )}
                                </div>

                                <div className="flex justify-end space-x-4 pt-6">
                                    <button
                                        type="button"
                                        className="px-6 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50 transition-colors"
                                        onClick={handleCancelPassword}
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="button"
                                        onClick={handleSavePassword}
                                        className="px-6 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
                                    >
                                        Change Password
                                    </button>
                                </div>
                            </div>
                        ) : (
                            /* Основная форма профиля */
                            <div className="space-y-6">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Name *
                                    </label>
                                    <input
                                        type="text"
                                        name="name"
                                        value={profile.name}
                                        onChange={handleInputChange}
                                        disabled={!isEditing}
                                        required
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            !isEditing 
                                                ? 'bg-gray-100 text-gray-500 cursor-not-allowed border-gray-300' 
                                                : errors.profile.name 
                                                    ? 'border-red-300 bg-red-50' 
                                                    : 'border-gray-300 bg-white'
                                        }`}
                                    />
                                    {errors.profile.name && (
                                        <p className="text-red-600 text-sm mt-1">{errors.profile.name}</p>
                                    )}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Surname *
                                    </label>
                                    <input
                                        type="text"
                                        name="surname"
                                        value={profile.surname}
                                        onChange={handleInputChange}
                                        disabled={!isEditing}
                                        required
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            !isEditing 
                                                ? 'bg-gray-100 text-gray-500 cursor-not-allowed border-gray-300' 
                                                : errors.profile.surname 
                                                    ? 'border-red-300 bg-red-50' 
                                                    : 'border-gray-300 bg-white'
                                        }`}
                                    />
                                    {errors.profile.surname && (
                                        <p className="text-red-600 text-sm mt-1">{errors.profile.surname}</p>
                                    )}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Phone
                                    </label>
                                    <input
                                        type="tel"
                                        name="phone"
                                        value={profile.phone}
                                        onChange={handleInputChange}
                                        disabled={!isEditing}
                                        className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${
                                            !isEditing 
                                                ? 'bg-gray-100 text-gray-500 cursor-not-allowed border-gray-300' 
                                                : errors.profile.phone 
                                                    ? 'border-red-300 bg-red-50' 
                                                    : 'border-gray-300 bg-white'
                                        }`}
                                    />
                                    {errors.profile.phone && (
                                        <p className="text-red-600 text-sm mt-1">{errors.profile.phone}</p>
                                    )}
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Email
                                    </label>
                                    <input
                                        type="email"
                                        name="email"
                                        value={profile.email}
                                        disabled
                                        className="w-full px-3 py-2 border border-gray-300 bg-gray-100 rounded-md text-gray-500 cursor-not-allowed"
                                    />
                                </div>

                                {/* Кнопки при редактировании */}
                                {isEditing && (
                                    <div className="flex justify-end space-x-4 pt-6">
                                        <button
                                            type="button"
                                            className="px-6 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50 transition-colors"
                                            onClick={handleCancel}
                                        >
                                            Cancel
                                        </button>
                                        <button
                                            type="button"
                                            onClick={handleSave} // или handleSaveAlternative
                                            className="px-6 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
                                        >
                                            Save Changes
                                        </button>
                                    </div>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Profile;