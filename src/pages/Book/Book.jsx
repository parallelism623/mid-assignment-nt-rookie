import {
  Space,
  Table,
  Tag,
  Card,
  Popconfirm,
  Row,
  Col,
  Button,
  Input,
  Tooltip,
} from "antd";
import { FiEdit, FiTrash2, FiEye } from "react-icons/fi";
import BookItemView from "../../components/ui/BookItemView";

import { defaultQueryParameters } from "../../constants/queryParameters";
import { ExclamationCircleOutlined } from "@ant-design/icons";
import { useEffect, useState } from "react";
import { bookServices } from "../../services/bookServices";
import { Navigate, useNavigate } from "react-router";
import { IoFilterSharp } from "react-icons/io5";
import BookFilter from "./BookFilters";
import "../../assets/styles/BookStyle.css";
import { environment } from "../../constants/environment";
import BookBorrowModal from "../Book/BookBorrowModal";
import { useMessageContext } from "../../components/context/MessageContext";

const { Search } = Input;
const MAX = environment.limit_books_per_request;
const columns = (onDelete, onEdit, onViewDetail) => [
  { key: "title", title: "Title", dataIndex: "title" },
  { key: "author", title: "Author", dataIndex: "author" },
  {
    key: "category",
    title: "Category",
    dataIndex: ["category", "name"],
    render: (value) => <Tag color="blue">{value}</Tag>,
    align: "center",
  },
  {
    key: "quantity",
    title: "Quantity",
    dataIndex: "quantity",
    render: (value) => <Tag color="yellow">{value}</Tag>,
    align: "center",
  },
  {
    key: "available",
    title: "Available",
    dataIndex: "available",
    render: (value) => {
      const isAvailable = value > 0;
      const style = {
        display: "inline-block",
        background: isAvailable ? "#F6FFEC" : "#FFF1F0",
        color: isAvailable ? "#52C41A" : "#FF4D4F",
        padding: "2px 8px",
        borderRadius: "5px",
        fontSize: "12px",
        fontWeight: 500,
        minWidth: "24px",
        textAlign: "center",
      };
      return (
        <div>
          <span style={style}>{isAvailable ? value : "Invailable"}</span>
        </div>
      );
    },
    align: "center",
  },
  {
    key: "action",
    title: "Action",
    dataIndex: "action",
    width: "1%",
    onHeaderCell: () => ({
      className: "be-vietnam-pro-medium",
    }),
    render: (_, record) => (
      <Space size="middle" className="!flex justify-center items-center">
        <Tooltip title="Edit">
          <button
            onClick={() => onEdit(record)}
            className="p-2 rounded-lg bg-blue-200 text-blue-800 hover:bg-blue-300 transition cursor-pointer"
          >
            <FiEdit size={18} />
          </button>
        </Tooltip>

        <Popconfirm
          placement="topRight"
          title="Are you sure to delete this book?"
          description="Once deleted, you will not be able to recover this book!"
          icon={<ExclamationCircleOutlined className="text-red-500 text-xl" />}
          overlayClassName="bg-white shadow-lg rounded-lg p-4 border border-gray-200"
          okText="Yes"
          cancelText="No"
          okButtonProps={{
            className:
              "bg-red-500 hover:bg-red-600 focus:ring-4 focus:ring-red-300 text-white font-medium rounded-lg text-sm px-4 py-2",
          }}
          cancelButtonProps={{
            className:
              "bg-gray-200 hover:bg-gray-300 focus:ring-4 focus:ring-gray-300 text-gray-700 font-medium rounded-lg text-sm px-4 py-2",
          }}
          onConfirm={() => onDelete(record)}
        >
          <Tooltip title="Delete">
            <button className="p-2 rounded-lg bg-red-300 text-red-800 hover:bg-red-400 transition cursor-pointer">
              <FiTrash2 size={18} />
            </button>
          </Tooltip>
        </Popconfirm>

        <Tooltip title="Detail">
          <button
            onClick={() => onViewDetail(record)}
            className="p-2 rounded-lg bg-emerald-300 text-emerald-800 hover:bg-emerald-400 transition cursor-pointer"
          >
            <FiEye size={18} />
          </button>
        </Tooltip>
      </Space>
    ),
    align: "center",
  },
];

const Book = () => {
  const [queryParameters, setQueryParameters] = useState(
    defaultQueryParameters
  );
  const [data, setData] = useState();
  const [bookId, setIsEdit] = useState(null);
  const [loading, setLoading] = useState(false);
  const [openFilter, setOpenFilter] = useState(false);
  const [borrowMode, setBorrowMode] = useState(false);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const { message } = useMessageContext();
  const [openBorrowBookModal, setOpenBorrowBookModal] = useState(false);
  const navigate = useNavigate();
  const onDelete = (record) => {
    setLoading(true);
    bookServices
      .deleteById(record.id)
      .then(() => {
        setQueryParameters({
          ...queryParameters,
          totalCount: queryParameters.totalCount - 1,
          pageIndex:
            (queryParameters.totalCount - 1) % queryParameters.pageSize === 0
              ? queryParameters.pageIndex - 1 < 1
                ? 1
                : queryParameters.pageIndex - 1
              : queryParameters.pageIndex,
        });
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
      });
  };
  const onSubmitFilter = (values) => {
    setQueryParameters(...queryParameters, values);
  };
  const onEdit = (record) => {
    setIsEdit(record.id);
  };
  const onSearch = (value) => {
    setQueryParameters({
      ...queryParameters,
      search: value,
    });
  };
  var columnsData = columns(onDelete, onEdit);
  const fetchData = (isSetData = true) => {
    setLoading(true);
    bookServices
      .gets(getBooksQueryParameters(queryParameters))
      .then((res) => {
        if (isSetData) {
          setData(res.items);
          setQueryParameters({
            pageIndex: res.pageIndex,
            pageSize: res.pageSize,
            totalCount: res.totalCount,
            search: queryParameters.search,
          });
        }
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
      });
  };
  useEffect(() => {
    let isSetData = true;
    fetchData(isSetData);
    return () => {
      isSetData = false;
    };
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.totalCount,
    queryParameters.search,
  ]);
  function handleChange(page, pageSize) {
    setQueryParameters({
      ...queryParameters,
      pageIndex: page,
      pageSize: pageSize,
    });
  }
  const handleOnClickBorrowBooks = () => {
    setBorrowMode(!borrowMode);
  };
  const handleOnSubmitBorrowBooks = () => {
    setBorrowMode(!borrowMode);
    if (selectedRowKeys.length > 0) {
      setOpenBorrowBookModal(true);
    }
  };
  const handleOnBorrowBooksChange = (selectedRowKeys) => {
    setSelectedRowKeys(selectedRowKeys);
    if (selectedRowKeys.length >= MAX) {
      message.info("You reach the limit of books you can borrow.");
    }
  };
  return (
    <>
      {setOpenBorrowBookModal && (
        <BookBorrowModal
          onSubmit={() => {
            fetchData();
          }}
          isModalOpen={openBorrowBookModal}
          setIsModalOpen={setOpenBorrowBookModal}
          bookIds={selectedRowKeys}
          setSelectBookIds={setSelectedRowKeys}
        />
      )}
      <BookFilter
        open={openFilter}
        onClose={() => setOpenFilter(false)}
        onSubmit={(value) => {
          onSubmitFilter(value);
        }}
      />
      {/* <BookItemView></BookItemView> */}
      {!!bookId && <Navigate to={`${bookId}/edit`} />}
      <div className="book-list-container">
        <Card
          extra={
            <Row className="flex items-center justify-between gap-1.5 overflow-x-hidden">
              <Col className="overflow-x-hidden">
                <Search
                  placeholder="Search book by title, author, or category"
                  onSearch={onSearch}
                  className="overflow-x-hidden"
                  style={{ width: 200 }}
                  defaultValue={queryParameters.search}
                />
              </Col>
              <Col>
                <Button
                  className="create-book-button"
                  onClick={() => navigate("/books/create")}
                >
                  Create Book
                </Button>
              </Col>
              <Col>
                {!borrowMode && (
                  <Button
                    onClick={handleOnClickBorrowBooks}
                    className="borrow-book-button hover:bg-"
                  >
                    Borrow Books
                  </Button>
                )}
                {borrowMode && (
                  <Button
                    className="submit-borrow-book-button"
                    onClick={handleOnSubmitBorrowBooks}
                  >
                    {selectedRowKeys.length === 0
                      ? "Cancel Borrow"
                      : "Submit Borrow"}
                  </Button>
                )}
              </Col>
              <Col
                className="flex items-center cursor-pointer p-2 hover:bg-gray-300 rounded-md"
                onClick={() => setOpenFilter(true)}
              >
                <span className="block mr-1">Filters</span>
                <IoFilterSharp className="text-lg text-gray-600 hover:text-gray-800 transition-colors cursor-pointer" />
              </Col>
            </Row>
          }
          title="Book List"
          className="book-list-card be-vietnam-pro-regular"
        >
          <Table
            rowSelection={
              borrowMode
                ? {
                    selectedRowKeys,
                    onChange: handleOnBorrowBooksChange,
                    hideSelectAll: true,
                    getCheckboxProps: (record) => {
                      const isSelected = selectedRowKeys.includes(record.id);
                      const reachedMax = selectedRowKeys.length >= MAX;

                      return {
                        disabled:
                          (reachedMax && !isSelected) || record.available === 0,
                      };
                    },
                  }
                : undefined
            }
            loading={loading}
            bordered
            className="book-list-table overflow-x-scroll"
            columns={columnsData.map((col) => {
              return {
                ...col,
                onHeaderCell: () => ({
                  className: "be-vietnam-pro-medium",
                }),
              };
            })}
            dataSource={data}
            rowKey={(record) => record.id}
            pagination={{
              current: queryParameters.pageIndex,
              total: queryParameters.totalCount,
              pageSize: queryParameters.pageSize,
              showSizeChanger: true,
              pageSizeOptions: [5, 10, 20, 50],
              showTotal: (total, [start, end]) =>
                `From ${start} to ${end} items of ${total}`,
              onChange: (p, ps) => {
                handleChange(p, ps);
              },
            }}
          />
        </Card>
      </div>
    </>
  );
};

export default Book;

const getBooksQueryParameters = (queryParameters) => {
  return {
    pageSize: queryParameters.pageSize,
    pageIndex: queryParameters.pageIndex,
    search: queryParameters.search,
  };
};
