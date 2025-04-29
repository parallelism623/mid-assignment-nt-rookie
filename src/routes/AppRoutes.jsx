import MainLayout from "../components/layout/MainLayout";
import Book from "../pages/Book/Book";
import Category from "../pages/Category/Category";
import Dashboard from "../pages/Dashboard/Dashboard";
import { routesPath } from "../constants/routesPath";
import User from "../pages/User/User";
import ProtectedRoutes from "./ProtectedRoute";
import { Navigate } from "react-router";
import Login from "../pages/Login/Login";
import BookEdit from "../pages/Book/BookEdit";
import BookCreate from "../pages/Book/BookCreate";
import BookBorrowingRequest from "../pages/BooksBorrowingRequest/BookBorrowingRequest";
import UserInfoCard from "../components/ui/UserInfoCard";
import Register from "../pages/Register/register";
import EmailConfirm from "../pages/EmailConfirm/email-confirm";
import BookBorrowed from "../pages/BookBorrowed/book-borrowed";

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
      { index: true, path: routesPath.dashBoard, element: <Dashboard /> },
      {
        path: routesPath.book,
        element: <Book />,
      },
      { path: routesPath.bookEdit, element: <BookEdit /> },
      { path: routesPath.bookCreate, element: <BookCreate /> },
      { path: routesPath.category, element: <Category /> },
      { path: routesPath.user, element: <User /> },
      { path: routesPath.bookBorrowing, element: <BookBorrowingRequest /> },
      { path: routesPath.userProfile, element: <UserInfoCard /> },
      { path: routesPath.bookBorrowed, element: <BookBorrowed /> },
    ],
  },
];
