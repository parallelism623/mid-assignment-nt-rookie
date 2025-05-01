import { Layout, Menu, Row, Col } from "antd";
import { MenuOutlined } from "@ant-design/icons";
import { FaBook, FaClipboardList, FaHistory } from "react-icons/fa";
import { useNavigate } from "react-router";
import { TbBasketFilled } from "react-icons/tb";
import "../../assets/styles/AppHeaderStyle.css";
import AccountMenu from "./../ui/AccountMenu";
import { useLocalStorage } from "../hooks/useStorage";
import { useUserContext } from "../../routes/ProtectedRoute";
import { environment } from "../../constants/environment";
const { Header } = Layout;

const AppHeader = () => {
  const navigate = useNavigate();
  const handleClickNav = ({ key }) => {
    navigate(key);
  };
  const { roleName } = useUserContext();
  const headerNav = [
    {
      key: "dashboard",
      label: "Dashboard",
      className: "header-nav-item be-vietnam-pro-light",
      visible: true,
    },
    {
      key: "categories",
      label: "Category",
      className: "be-vietnam-pro-light header-nav-item",
      visible: roleName === environment.adminRole,
    },
    {
      key: "books",
      label: "Book",
      className: "be-vietnam-pro-light header-nav-item",
      children: [
        { key: "/books", label: "All Books", icon: <FaBook /> },
        {
          key: "/books-borrowing",
          label: "Books Borrowing Request",
          icon: <FaClipboardList />,
        },
        { key: "/book-borrowed", label: "Book Borrowed", icon: <FaHistory /> },
      ],
      visible: true,
    },
    {
      key: "users",
      label: "User",
      visible: roleName === environment.adminRole,
      className: "be-vietnam-pro-light header-nav-item",
    },
  ];
  return (
    <>
      <Header className="app-header">
        <Row style={{ width: "100%" }} gap={"0.5em"}>
          <Col
            span={1}
            style={{
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            <div className="header-logo">
              <FaBook />
            </div>
          </Col>
          <Col span={18}>
            <Menu
              onClick={handleClickNav}
              overflowedIndicator={
                <span>
                  <MenuOutlined
                    style={{
                      color: "var(--font-color-dark-mode)",
                      fontSize: "2.2em",
                      lineHeight: 1,
                    }}
                  />
                </span>
              }
              style={{
                display: "flex",
                backgroundColor: "inherit",
                inlineCollapsed: false,
                width: "",
              }}
              mode="horizontal"
              defaultSelectedKeys={["home"]}
              items={headerNav.filter((hn) => hn.visible === true)}
            />
          </Col>
          <Col span={5}>
            <Row
              justify="end"
              align="middle"
              wrap
              gutter={[8, 8]}
              className="row-container-flex-sub-nav"
            >
              <Col
                xs={12}
                sm={8}
                md={6}
                lg={5}
                xl={4}
                xxl={4}
                className="col-container-flex-sub-nav"
              >
                <AccountMenu />
              </Col>
            </Row>
          </Col>
        </Row>
      </Header>
    </>
  );
};

export default AppHeader;
