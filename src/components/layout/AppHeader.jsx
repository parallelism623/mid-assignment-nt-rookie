import { Layout, Menu, Row, Col } from "antd";
import { MenuOutlined } from "@ant-design/icons";
import { FaBook, FaClipboardList, FaHistory } from "react-icons/fa";
import { useNavigate } from "react-router";
import "../../assets/styles/AppHeaderStyle.css";
import AccountMenu from "./../ui/AccountMenu";

const { Header } = Layout;
const headerNav = [
  {
    key: "dashboard",
    label: "Dashboard",
    className: "header-nav-item be-vietnam-pro-light",
  },
  {
    key: "categories",
    label: "Category",
    className: "be-vietnam-pro-light header-nav-item",
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
  },
  {
    key: "users",
    label: "User",
    className: "be-vietnam-pro-light header-nav-item",
  },
];
const AppHeader = () => {
  const navigate = useNavigate();
  const handleClickNav = ({ key }) => {
    navigate(key);
  };
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
              items={headerNav}
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
