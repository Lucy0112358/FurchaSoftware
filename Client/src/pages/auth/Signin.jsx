import React, { useState } from 'react'
import './auth.css';


const Signin = () => {
    const [formData, setFormData] = useState({
        email: '',
        password: '',
      });

      const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
          ...formData,
          [name]: value,
        });
      };

      const handleSubmit = (e) => {
        e.preventDefault();
        console.log('Email:', formData.email);
        console.log('Password:', formData.password);
      };

    return (
        <div className="signin__container flex items-center justify-center min-h-screen">
            <div className="w-96">
                <h2 className="text-2xl font-bold mb-6 text-center">Sign In</h2>
                <form className='p-4'>
                    <div className="mb-4">
                        <label className="signin__container__text" htmlFor="email">
                            Email
                        </label>
                        <input
                            id="email"
                            type="email"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300"
                            placeholder="Enter your email"
                        />
                    </div>
                    <div className="mb-6">
                        <label className="signin__container__text" htmlFor="password">
                            Password
                        </label>
                        <input
                            id="password"
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            className="w-full px-3 py-2 text-gray-700 border rounded focus:outline-none focus:ring focus:border-blue-300"
                            placeholder="Enter your password"
                        />
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
                            className="signin__container__text inline-block align-baseline "
                        >
                            Forgot Password?
                        </a>
                    </div>
                </form>
            </div>
        </div>
    )
}

export default Signin