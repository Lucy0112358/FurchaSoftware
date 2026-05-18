# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
npm run dev        # Development server (port 5173)
npm run build      # Production build
npm run lint       # ESLint check
npm run preview    # Preview production build
```

No test framework is configured.

## Architecture

**Furcha** is a React 18 + Redux Toolkit SPA for managing lockers, users, admins, branches, and modules. Uses Vite + SWC, Tailwind CSS + SCSS, React Router v6, Formik + Yup, and SignalR for real-time locker updates.

### Folder Layout

```
src/
├── pages/          # Top-level page components per domain
├── components/
│   ├── tables/     # Data tables per domain
│   ├── modals/     # Modals organized by domain subfolder
│   ├── popups/     # Right-click context menus
│   └── layout/     # Sidebar + main layout wrapper
├── redux/
│   ├── api/        # createAsyncThunk API calls per domain
│   ├── slice/      # Redux slices with extraReducers
│   └── store/      # Combined store
├── hooks/          # useContextMenu, useHasPermission
├── middleware/     # PrivateRoute (auth guard)
├── config/axios/   # Axios instance with JWT interceptor
├── enums/Locker/   # Locker types, statuses, sizes
└── App.jsx         # Router config
```

### Redux Pattern

Each domain (user, locker, branch, module, admin) follows the same pattern:

1. `redux/api/[domain]Api.js` — `createAsyncThunk` functions calling the Axios instance
2. `redux/slice/[domain]Slice.js` — slice with `extraReducers` handling pending/fulfilled/rejected; exports selectors
3. `redux/store/store.js` — registers all slices

### Authentication

- Login → JWT stored in `localStorage` → Axios interceptor attaches `Bearer` token to every request
- `PrivateRoute` checks for token and dispatches `getAuthUser()` on mount
- Axios response interceptor clears token and redirects to `/login` on 401
- `authUserPermissions` in auth slice holds role + permissions array

### Key Conventions

- Components: `.jsx`; Redux slices/API: `.js`
- No TypeScript — pure JavaScript/JSX project
- Per-component CSS/SCSS files alongside the component (`user.css`, `layout.css`, etc.)
- Environment variables accessed via `import.meta.env.VITE_*` (defined in `.env`)
- API errors expected in shape `{ error: { both: "message" } }`
- Toasts via React Toastify; loading state via `<Loader />`; empty state via `<NoData />`

### Locker Domain

Core domain with enums in `src/enums/Locker/`:
- **Types**: personal, common, handover, parcel, unspecified (each has a hex color via `getLockerColor()` in `Utils.js`)
- **Status**: free (1), occupied (2)
- **Sizes**: S, M, L, XL, XXL, XXXL

### Adding Features

**New page**: create `src/pages/[domain]/`, add Redux slice + API thunks, register in store, add route in `App.jsx` inside `<PrivateRoute>`.

**New modal**: create `src/components/modals/[domain]/`, use Formik for the form, dispatch async thunks.

**New table**: `src/components/tables/`, use `useSelector` for data, `useContextMenu` hook for right-click actions, multi-select with local `useState`.
