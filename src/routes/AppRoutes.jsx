import MainLayout from "../components/layout/MainLayout";
import Book from "../pages/book/Book";
import Category from "../pages/Category/Category";
import Dashboard from "../pages/Dashboard/Dashboard";
import { routesPath } from "../constants/routesPath";
import User from "../pages/User/User";
import ProtectedRoutes from "./ProtectedRoute";
import { Navigate } from "react-router";
import Login from "../pages/Login/Login";
import BookEdit from "../pages/book/book-edit";
import BookCreate from "../pages/book/book-create";
import BookBorrowingRequest from "../pages/BooksBorrowingRequest/BookBorrowingRequest";
import UserProfile from "../pages/UserProfile/UserProfile";
import Register from "../pages/Register/register";
import EmailConfirm from "../pages/EmailConfirm/email-confirm";
import BookBorrowed from "../pages/BookBorrowed/book-borrowed";
import BookDetail from "../pages/book/book-detail";
import ForbiddenPage from "../pages/Forbidden/forbidden";
import AdminRoutes from "./AdminRoutes";
import Report from "../pages/Reports/Report";
export const appRoutes = [
  { path: "*", element: <Navigate to={routesPath.home} replace /> },
  { path: routesPath.signIn, element: <Login /> },
  { path: routesPath.register, element: <Register /> },
  { path: routesPath.emailConfirm, element: <EmailConfirm /> },
  {
    path: routesPath.home,
    element: (
      <ProtectedRoutes>
        <MainLayout />
      </ProtectedRoutes>
    ),
    children: [
      {
        index: true,
        element: <Navigate to={routesPath.book} replace />,
      },

      {
        index: true,
        path: routesPath.book,
        element: <Book />,
      },
      {
        element: <AdminRoutes />,
        children: [
          { path: routesPath.bookEdit, element: <BookEdit /> },
          { path: routesPath.bookCreate, element: <BookCreate /> },
          { path: routesPath.user, element: <User /> },
          { path: routesPath.category, element: <Category /> },
          { path: routesPath.reports, element: <Report /> },
        ],
      },
      { path: routesPath.bookBorrowing, element: <BookBorrowingRequest /> },
      { path: routesPath.userProfile, element: <UserProfile /> },
      { path: routesPath.bookBorrowed, element: <BookBorrowed /> },
      { path: routesPath.bookDetail, element: <BookDetail /> },
    ],
  },
  {
    path: routesPath.forbidden,
    element: <ForbiddenPage />,
  },
];
