import React from 'react';
import { useNavigate } from 'react-router-dom';

const NotFound = () => {
  const navigate = useNavigate();

  const handleGoBack = () => {
    navigate(-1);
  };

  return (
    <div className="h-screen flex items-center justify-center flex-col">
      <main className="grid min-h-full place-items-center px-6 py-24 sm:py-32 lg:px-8">
        <div className="text-center">
          <h1 className="mt-4 text-5xl font-semibold tracking-tight text-balance text-gray-300 sm:text-7xl">
            Page not found
          </h1>
          <p className="mt-6 text-lg font-medium text-pretty text-gray-300 sm:text-xl/8">
            Sorry, we couldn’t find the page you’re looking for.
          </p>
          <button
            onClick={handleGoBack}
            className="mt-8 inline-flex items-center rounded-md bg-gray-500 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-gray-400 transition"
          >
            Go Back
          </button>
        </div>
      </main>
    </div>
  );
};

export default NotFound;