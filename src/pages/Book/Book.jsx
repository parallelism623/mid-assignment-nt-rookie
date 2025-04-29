import {
  Card,
  Row,
  Col,
  Button,
  Input,
  Tag,
  Checkbox,
  Pagination,
  Space,
  Popconfirm,
  Rate,
  Tooltip,
} from "antd";

import {
  FiStar,
  FiEdit,
  FiTrash2,
  FiEye,
  FiTag,
  FiLayers,
  FiXCircle,
  FiCheckCircle,
} from "react-icons/fi";
import { ExclamationCircleOutlined } from "@ant-design/icons";
import { IoFilterSharp } from "react-icons/io5";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router";
import FractionalStar from "../../components/ui/Icons/FractionalStar";
import { bookServices } from "../../services/bookServices";
import BookBorrowModal from "../Book/BookBorrowModal";
import BookFilter from "./BookFilters";
import "../../assets/styles/BookStyle.css";
import { defaultQueryParameters } from "../../constants/queryParameters";
import { environment } from "../../constants/environment";
import { useMessageContext } from "../../components/context/MessageContext";
import ReviewButton from "../../components/ui/Buttons/ReviewButton";
import EditButton from "../../components/ui/Buttons/EditButton";
import DeleteButton from "../../components/ui/Buttons/DeleteButton";
import ViewDetailButton from "../../components/ui/Buttons/ViewDetailButton";
import BookReviewCardModal from "./BookReviewCardModal";
import { useUserContext } from "../../routes/ProtectedRoute";
const { Search } = Input;
const MAX = environment.limit_books_per_request;

const Book = () => {
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameters,
    pageSize: 8,
  });
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [openFilter, setOpenFilter] = useState(false);
  const [borrowMode, setBorrowMode] = useState(false);
  const selectedRowKeysRef = useRef([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState(
    selectedRowKeysRef.current
  );

  const [openBorrowBookModal, setOpenBorrowBookModal] = useState(false);
  const [openBookReview, setOpenBookReview] = useState(false);
  const [selectedBook, setSelectedBook] = useState(null);
  const { message } = useMessageContext();
  const { id, roleName, refreshUser } = useUserContext();
  const navigate = useNavigate();

  const fetchData = (isSetData = true) => {
    setLoading(true);
    bookServices
      .gets({
        pageSize: queryParameters.pageSize,
        pageIndex: queryParameters.pageIndex,
        search: queryParameters.search,
      })
      .then((res) => {
        if (isSetData) {
          setData(res.items);
          setQueryParameters({
            ...queryParameters,
            pageIndex: res.pageIndex,
            pageSize: res.pageSize,
            totalCount: res.totalCount,
          });
        }
        setLoading(false);
      })
      .catch(() => setLoading(false));
  };
  useEffect(() => {
    let isActive = true;
    fetchData(isActive);
    return () => {
      isActive = false;
    };
  }, [
    queryParameters.pageIndex,
    queryParameters.pageSize,
    queryParameters.search,
  ]);
  const actionOfBookItemsUser = [
    <ReviewButton
      size={15}
      onClick={(record) => {
        handleOnClickReviewBookButton(record);
      }}
    />,
    <ViewDetailButton size={15} onClick={(record) => onViewDetail(record)} />,
  ];
  const actionOfBookItemsAdmin = [
    <EditButton
      size={15}
      onClick={(record) => navigate(`${record.id}/edit`)}
    />,
    <Popconfirm
      key="delete"
      centered
      title="Are you sure to delete?"
      icon={<ExclamationCircleOutlined />}
      onConfirm={(record) => onDelete(record)}
    >
      <DeleteButton size={15} />
    </Popconfirm>,
    <ViewDetailButton size={15} onClick={(record) => onViewDetail(record)} />,
  ];

  const handleChange = (page, pageSize) => {
    setQueryParameters({ ...queryParameters, pageIndex: page, pageSize });
  };

  const onSearch = (value) => {
    setQueryParameters({ ...queryParameters, search: value });
  };

  const onDelete = (record) => {
    setLoading(true);
    bookServices
      .deleteById(record.id)
      .then(() => {
        const newTotal = queryParameters.totalCount - 1;
        setQueryParameters({
          ...queryParameters,
          totalCount: newTotal,
          pageIndex:
            newTotal % queryParameters.pageSize === 0
              ? Math.max(1, queryParameters.pageIndex - 1)
              : queryParameters.pageIndex,
        });
        setLoading(false);
      })
      .catch(() => setLoading(false));
  };

  const toggleBorrowMode = () => {
    setBorrowMode(!borrowMode);
    if (borrowMode) {
      setSelectedRowKeys([]);
      selectedRowKeysRef.current = [];
    }
  };
  const handleSubmitBorrow = () => {
    if (selectedRowKeys.length) setOpenBorrowBookModal(true);
    else setBorrowMode(false);
  };
  const handleOnBorrowBooksChange = (newKeys) => {
    setSelectedRowKeys(newKeys);
    selectedRowKeysRef.current = newKeys;
    if (newKeys.length >= MAX) {
      message.info("You reach limit books per request");
    }
  };

  const toggleSelect = (id) => {
    const isSel = selectedRowKeys.includes(id);
    const newKeys = isSel
      ? selectedRowKeys.filter((k) => k !== id)
      : [...selectedRowKeys, id];
    handleOnBorrowBooksChange(newKeys);
  };
  const handleOnCancelBookReview = () => {
    setOpenBookReview(false);
    setSelectedBook(null);
  };
  const handleOnClickReviewBookButton = (book) => {
    setOpenBookReview(true);
    setSelectedBook(book);
  };

  return (
    <>
      <BookFilter
        open={openFilter}
        onClose={() => setOpenFilter(false)}
        onSubmit={(vals) => setQueryParameters({ ...queryParameters, ...vals })}
      />
      <BookBorrowModal
        isModalOpen={openBorrowBookModal}
        bookIds={selectedRowKeys}
        onSubmit={() => {
          fetchData();
          refreshUser();
        }}
        onCancel={() => {
          setSelectedRowKeys([]);
          selectedRowKeysRef.current = [];
          setBorrowMode(false);
          setOpenBorrowBookModal(false);
        }}
      />
      {openBookReview && (
        <BookReviewCardModal
          visible={openBookReview}
          onCancel={handleOnCancelBookReview}
          onSubmit={() => {
            fetchData();
          }}
          book={selectedBook}
          reviewerId={id}
        />
      )}
      <Card
        loading={loading}
        title="Book List"
        extra={
          <Row className="flex items-center gap-2">
            <Col>
              <Search
                placeholder="Search by title/author/category"
                onSearch={onSearch}
                style={{ width: 240 }}
                defaultValue={queryParameters.search}
                loading={loading}
              />
            </Col>
            <Col>
              <Button onClick={() => navigate("/books/create")}>
                Create Book
              </Button>
            </Col>
            <Col>
              {!borrowMode ? (
                <Button onClick={toggleBorrowMode}>Borrow Books</Button>
              ) : (
                <Button onClick={handleSubmitBorrow}>
                  {selectedRowKeys.length ? "Submit Borrow" : "Cancel Borrow"}
                </Button>
              )}
            </Col>
            <Col
              className="flex items-center cursor-pointer"
              onClick={() => setOpenFilter(true)}
            >
              Filters <IoFilterSharp className="ml-1" />
            </Col>
          </Row>
        }
        className="book-list-card"
      >
        <Row gutter={[16, 16]}>
          {data.map((record) => {
            const isAvailable = record?.available > 0;
            const isSelected = selectedRowKeys?.includes(record.id);
            const reachedMax = selectedRowKeys?.length >= MAX && !isSelected;
            const disabled = reachedMax || !isAvailable;
            const availStyle = {
              display: "inline-block",
              background: isAvailable ? "#F6FFEC" : "#FFF1F0",
              color: isAvailable ? "#52C41A" : "#FF4D4F",
              padding: "2px 8px",
              borderRadius: "5px",
              fontSize: "12px",
              minWidth: "24px",
              textAlign: "center",
            };

            return (
              <Col key={record.id} xs={24} sm={12} md={8} lg={6}>
                <Card
                  hoverable
                  cover={
                    <img
                      alt={record.title}
                      src={
                        record.coverUrl ||
                        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ2gtz3pCsopRPlbimr8KOduu3nnEFxAzTdHw&s"
                      }
                      className="h-48 w-full object-cover"
                    />
                  }
                  title={
                    <>
                      <h3>{record.title}</h3>
                      <p className="text-sm text-gray-600">
                        by {record.author}
                      </p>
                    </>
                  }
                  extra={
                    borrowMode && (
                      <Checkbox
                        checked={isSelected}
                        disabled={disabled}
                        onChange={() => toggleSelect(record.id)}
                      />
                    )
                  }
                  actions={
                    roleName === environment.adminRole
                      ? actionOfBookItemsAdmin
                      : actionOfBookItemsUser
                  }
                  className={isSelected ? "ring-2 ring-blue-400" : ""}
                >
                  <Card.Meta
                    description={
                      <>
                        <div className="text-gray-700">
                          {record.description}
                        </div>
                      </>
                    }
                  />
                  <div className="mt-4 space-y-2">
                    <div className="flex items-center space-x-2">
                      <FiTag className="text-gray-500" />
                      <span className="font-medium text-gray-700">
                        Category:
                      </span>
                      <Tag color="blue">{record.category.name}</Tag>
                    </div>

                    <div className="flex items-center space-x-2">
                      <FiLayers className="text-gray-500" />
                      <span className="font-medium text-gray-700">
                        Quantity:
                      </span>
                      <Tag color="yellow">{record.quantity}</Tag>
                    </div>

                    <div className="flex items-center space-x-2">
                      {isAvailable ? (
                        <FiCheckCircle className="text-green-500" />
                      ) : (
                        <FiXCircle className="text-red-500" />
                      )}
                      <span className="font-medium text-gray-700">
                        Available:
                      </span>
                      <span style={availStyle}>
                        {isAvailable ? record.available : "Invailable"}
                      </span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <FractionalStar
                        rating={record.averageRating.toFixed(1)}
                        size={18}
                      ></FractionalStar>
                      <span className="font-medium text-gray-700">
                        Average rating:
                      </span>
                      <span className="text-gray-800 font-semibold">
                        {record.averageRating.toFixed(1)} (
                        {record.numberOfReview} reviews)
                      </span>
                    </div>
                  </div>
                </Card>
              </Col>
            );
          })}
        </Row>
        <div className="mt-6 flex justify-end">
          <Pagination
            current={queryParameters.pageIndex}
            pageSize={queryParameters.pageSize}
            total={queryParameters.totalCount}
            showSizeChanger
            pageSizeOptions={[4, 8, 12, 20, 40, 100]}
            onChange={handleChange}
            showTotal={(t, [start, end]) => `From ${start} to ${end} of ${t}`}
          />
        </div>
      </Card>
    </>
  );
};

export default Book;
