import React, { useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { signin } from '../../redux/api/authApi';
import { useNavigate } from "react-router-dom";
import { Formik, Form, Field, ErrorMessage, useField } from 'formik';
import * as Yup from 'yup';
import './auth.css';
import { toast } from 'react-toastify';
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { getLoading, setLoading } from '../../redux/slice/authSlice';
import Loader from '../../components/loader/Loader';

const Signin = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const loading = useSelector(getLoading);
    const [showPassword, setShowPassword] = useState(false);


    const validationSchema = Yup.object({
        email: Yup.string().email('Wrong email').required('Email is required'),
        password: Yup.string()
            .min(6, 'Password must be at least 6 characters')
            .required('Password is required'),
    });

    const handleSubmit = async (values) => {
        dispatch(setLoading(true))
        try {
            await dispatch(signin(values)).unwrap();
            navigate('/users');
            toast.success("Welcome to Furcha");
        } catch (error) {
            toast.error("Error: " + (error.message || "Unknown error"));
        }
        finally {
            dispatch(setLoading(false))
        }
    };

    return (
        <div className="signin__container flex items-center justify-center min-h-screen">
            {loading ? (<Loader />) :
                <div className="w-96">
                    <h2 className="text-2xl font-bold mb-6 text-center">Sign In</h2>
                    <Formik
                        initialValues={{ email: 'user7@example.com', password: '123456' }}
                        validationSchema={validationSchema}
                        onSubmit={handleSubmit}
                    >
                        {({ handleChange, values }) => (
                            <Form className="p-4">
                                <div className="mb-4">
                                    <label className="signin__container__text" htmlFor="email">
                                        Email
                                    </label>
                                    <Field
                                        id="email"
                                        type="email"
                                        name="email"
                                        className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300"
                                        placeholder="Enter your email"
                                    />
                                    <ErrorMessage name="email" component="div" className="text-red-500 text-sm mt-1" />
                                </div>
                                <div className="mb-6 relative">
                                    <label className="signin__container__text" htmlFor="password">
                                        Password
                                    </label>
                                    <Field name="password">
                                        {({ field }) => (
                                            <div className="relative">
                                                <input
                                                    {...field}
                                                    type={showPassword ? 'text' : 'password'}
                                                    id="password"
                                                    className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300 pr-10"
                                                    placeholder="Enter your password"
                                                />
                                                <button
                                                    type="button"
                                                    onClick={() => setShowPassword(!showPassword)}
                                                    className="absolute top-2.5 right-3 text-gray-500"
                                                >
                                                    {showPassword ? <FaEyeSlash size={20} /> : <FaEye size={20} />}
                                                </button>
                                            </div>
                                        )}
                                    </Field>
                                    <ErrorMessage name="password" component="div" className="text-red-500 text-sm mt-1" />
                                </div>
                                <div className="signin__container__action flex items-center justify-between">
                                    <button
                                        type="submit"
                                        className="signin__container__action__button signin__container__text py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                                    >
                                        Sign In
                                    </button>
                                    <a
                                        href="#"
                                        className="signin__container__text inline-block align-baseline"
                                    >
                                        Forgot Password?
                                    </a>
                                </div>
                            </Form>
                        )}
                    </Formik>
                </div>
            }
        </div>
    );
};

export default Signin;
