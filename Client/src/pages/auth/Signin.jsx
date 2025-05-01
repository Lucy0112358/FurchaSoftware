import React from 'react';
import { useDispatch } from 'react-redux';
import { signin } from '../../redux/api/authApi';
import { useNavigate } from "react-router-dom";
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import './auth.css';
import { toast } from 'react-toastify';

const Signin = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const validationSchema = Yup.object({
        email: Yup.string().email('Wrong email').required('Email is required'),
        password: Yup.string()
            .min(6, 'Password must be at least 6 characters')
            .required('Password is required'),
    });

    const handleSubmit = async (values) => {
        try {
            await dispatch(signin(values)).unwrap();
            navigate('/users');
            toast.success("Welcome to Furcha");
        } catch (error) {
            toast.error("Error: " + (error.message || "Unknown error"));
        }
    };

    return (
        <div className="signin__container flex items-center justify-center min-h-screen">
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
                                    value='user7@example.com'
                                    className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300"
                                    placeholder="Enter your email"
                                />
                                <ErrorMessage name="email" component="div" className="text-red-500 text-sm mt-1" />
                            </div>
                            <div className="mb-6">
                                <label className="signin__container__text" htmlFor="password">
                                    Password
                                </label>
                                <Field
                                    id="password"
                                    type="password"
                                    name="password"
                                    value='123456'
                                    className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300"
                                    placeholder="Enter your password"
                                />
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
        </div>
    );
};

export default Signin;
