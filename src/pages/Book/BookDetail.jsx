import React, { useEffect, useState } from "react";
import {
  Card,
  Row,
  Col,
  Image,
  Tag,
  Typography,
  List,
  Rate,
  Button,
  Upload,
  Space,
} from "antd";
import {
  FiUser,
  FiTag,
  FiInfo,
  FiCheckCircle,
  FiBox,
  FiClock,
  FiChevronDown,
} from "react-icons/fi";
import dayjs from "dayjs";
const { Paragraph, Title } = Typography;
import { bookServices } from "../../services/bookServices";
import { useParams, useNavigate } from "react-router";
import BookReviewCardModal from "../Book/BookReviewCardModal";
import { VscCodeReview } from "react-icons/vsc";
import FractionalStar from "../../components/ui/icons/FractionalStar";
import { bookReviewServices } from "../../services/bookReviewServices";
import EditButtonText from "../../components/ui/buttons/EditButtonText";
import DeleteButtonText from "../../components/ui/buttons/DeleteButtonText";
import ReviewButtonText from "../../components/ui/buttons/ReviewButtonText";
import { useUserContext } from "../../routes/ProtectedRoute";
import ConfirmModal from "../../components/ui/ConfirmModal";
import { environment } from "../../constants/environment";

const BookDetail = () => {
  const { id } = useParams();
  const [book, setBook] = useState(null);
  const [reviewsData, setReviewsData] = useState([]);
  const navigate = useNavigate();
  const userInfo = useUserContext();
  const [openBookReview, setOpenBookReview] = useState(false);
  const [openDeleteConfirmPopup, setOpenDeleteConfirmPopup] = useState(false);
  const defaultQueryParameters = {
    skip: 0,
    take: 3,
    totalCount: 4,
    bookId: id,
    rating: [1, 2, 3, 4, 5],
  };
  const [queryParameters, setQueryParameters] = useState({
    ...defaultQueryParameters,
  });

  let {
    title,
    author,
    category,
    averageRating,
    description,
    available,
    quantity,
    numberOfReview,
    subImagesUrlSigned,
    imageUrlSigned,
    imageUrl,
    subImagesUrl = [],
  } = book ?? [];
  const fetchData = (isSetData = true) => {
    bookServices.getById(id).then((res) => {
      if (isSetData == true) {
        setBook(res);
      }
    });
  };
  const fetchDataBookReviews = (fetchDataParameters, isSetData = true) => {
    bookReviewServices.get(fetchDataParameters).then((res) => {
      if (fetchDataParameters.skip >= fetchDataParameters.totalCount) return;
      if (isSetData) {
        setQueryParameters({
          ...fetchDataParameters,
          totalCount: res.totalCount,
        });
        setReviewsData([...reviewsData, ...res.items]);
      }
    });
  };
  const resetBookReviews = (fetchDataParameters, isSetData = true) => {
    setReviewsData([]);
    setQueryParameters({
      ...fetchDataParameters,
    });
  };
  useEffect(() => {
    let isSetDate = true;
    fetchData(isSetDate);
    return () => {
      isSetDate = false;
    };
  }, [id]);
  useEffect(() => {
    let isSetData = true;
    fetchDataBookReviews(queryParameters, isSetData);

    return () => {
      isSetData = false;
    };
  }, [queryParameters.skip, queryParameters.take, queryParameters.rating]);
  const handleOnClickExpand = () => {
    setQueryParameters({ ...queryParameters, skip: queryParameters.skip + 3 });
  };
  let actions = [
    <Button
      key="expand"
      type="text"
      onClick={() => handleOnClickExpand()}
      icon={<FiChevronDown />}
      style={{ fontWeight: "bold" }}
    >
      Expand
    </Button>,
  ];
  if (queryParameters.skip + 3 >= queryParameters.totalCount) {
    actions = [];
  }

  const handleOnConfirmDeleteBook = () => {
    setOpenDeleteConfirmPopup(false);
    bookServices.deleteById(book.id).then(() => {
      setBook(null);
      navigate(-1);
    });
  };

  const handleOnCancelBookReview = () => {
    setOpenBookReview(false);
  };
  const handleOnClickBOokReViewButton = () => {
    setOpenBookReview(true);
  };
  const handleOnSubmittedBookReview = () => {
    setOpenBookReview(false);
  };
  return (
    <>
      {openBookReview && (
        <BookReviewCardModal
          visible={openBookReview}
          onCancel={handleOnCancelBookReview}
          book={book}
          reviewerId={userInfo.id}
          onSubmit={() => {
            handleOnSubmittedBookReview();
            fetchData();
            resetBookReviews({ ...defaultQueryParameters });
          }}
        />
      )}
      {openDeleteConfirmPopup && (
        <ConfirmModal
          title={"Confirm Book Deletion"}
          message={`Are you sure you want to delete the book ${book?.title}? This action cannot be undone`}
          visitable={openDeleteConfirmPopup}
          onCancel={() => {
            setOpenDeleteConfirmPopup(false);
          }}
          onSubmit={() => {
            handleOnConfirmDeleteBook();
          }}
          submitText={"Submit"}
        />
      )}
      <div className="flex justify-center">
        <Card
          hoverable
          title={<h2 className="text-2xl font-bold my-4">{title}</h2>}
          center
          className="rounded-2xl shadow-lg p-6 !w-min-40 !w-250"
          extra={
            <>
              <Space>
                {userInfo.roleName !== environment.adminRole && (
                  <ReviewButtonText
                    onClick={handleOnClickBOokReViewButton}
                  ></ReviewButtonText>
                )}
                {userInfo.roleName === environment.adminRole && (
                  <EditButtonText
                    onClick={() => navigate(`/books/${book.id}/edit`)}
                  />
                )}
                {userInfo.roleName === environment.adminRole && (
                  <DeleteButtonText
                    onClick={() => setOpenDeleteConfirmPopup(true)}
                  />
                )}
              </Space>
            </>
          }
        >
          <Row gutter={32}>
            <Col md={14} sm={24}>
              <Card
                bordered={false}
                style={{
                  borderRadius: 16,
                  background: "#fff",
                  boxShadow: " 0 8px 24px rgba(0,0,0,0.12)",
                }}
                className="!w-fit !h-fit max-w-125"
                bodyStyle={{ padding: 24 }}
              >
                <div className="w-fit h-min-80 h-fit mb-4 overflow-hidden rounded-lg bg-gray-100">
                  <Image
                    src={imageUrlSigned}
                    alt={title}
                    className="rounded-lg mb-4 !h-100 !w-120 !object-contain"
                  />
                </div>

                <div
                  className="
                      flex flex-nowrap        
                      overflow-x-scroll       
                      space-x-3           
                      pb-2                    
                    "
                >
                  {subImagesUrlSigned?.slice(0, 12).map((img, i) => (
                    <div
                      key={i}
                      className="
                            w-20 h-20 
                            flex-shrink-0          
                            overflow-hidden 
                            rounded-lg 
                            cursor-pointer 
                            hover:ring-2 hover:ring-blue-400
                          "
                    >
                      <Image
                        src={img}
                        alt={`${title} thumbnail ${i + 1}`}
                        rootClassName="w-full h-full"
                        imgClassName="w-full h-full object-cover"
                      />
                    </div>
                  ))}
                </div>
              </Card>
            </Col>
            <Col md={10} sm={24}>
              <div className="space-y-4 text-gray-700">
                <div className="flex items-center space-x-2">
                  <FiUser className="text-lg" />
                  <span className="font-medium">Author:</span>
                  <span>{author}</span>
                </div>

                <div className="flex items-center space-x-2">
                  <FiTag className="text-lg" />
                  <span className="font-medium">Category:</span>
                  <Tag color="blue">{category?.name}</Tag>
                </div>

                <div className="flex items-center space-x-2">
                  <FractionalStar
                    size={16}
                    rating={averageRating?.toFixed(1)}
                  />
                  <span className="font-medium">Average Rating:</span>
                  <span className="ml-1 text-sm font-semibold">
                    {averageRating?.toFixed(1)}
                  </span>
                </div>
                <div className="flex items-center space-x-2">
                  <VscCodeReview className="text-lg" />
                  <span className="font-medium">Number of reviews:</span>
                  <span className="ml-1 text-sm font-semibold">
                    {numberOfReview}
                  </span>
                </div>

                <div className="flex items-center space-x-2">
                  <FiCheckCircle className="text-lg" />
                  <span className="font-medium">Available:</span>
                  {available > 0 ? (
                    <Tag color="green">{available}</Tag>
                  ) : (
                    <Tag color="red">Unavailable</Tag>
                  )}
                </div>
                {userInfo.roleName === environment.adminRole && (
                  <div className="flex items-center space-x-2">
                    <FiBox className="text-lg" />
                    <span className="font-medium">Quantity:</span>
                    <Tag color="gold">{quantity}</Tag>
                  </div>
                )}

                <div className="space-y-2">
                  <div className="flex items-center space-x-2">
                    <FiInfo className="text-lg mt-[3px]" />
                    <span className="font-medium">Description:</span>
                  </div>

                  <Paragraph
                    className="px-4 py-2 bg-gray-50 rounded-lg overflow-y-scroll  max-h-48"
                    ellipsis={{
                      rows: 4,
                      expandable: true,
                      symbol: "More",
                    }}
                  >
                    {description || "—"}
                  </Paragraph>
                </div>
              </div>
            </Col>
          </Row>

          <Row>
            {!!numberOfReview && (
              <Card
                title={
                  <>
                    <p className="text-2xl">Book reviews</p>
                  </>
                }
                bordered={false}
                bodyStyle={{ padding: 24 }}
                actions={actions}
                className="w-full !mt-5"
              >
                <List
                  className="!min-w-180"
                  itemLayout="vertical"
                  dataSource={reviewsData}
                  renderItem={(item) => (
                    <List.Item className="pb-6 border-b last:border-b-0">
                      <div className="flex justify-between items-center mb-2">
                        <div className="flex items-center space-x-2">
                          <span className="font-medium">
                            {item?.reviewer?.username || "Unknown"}
                          </span>
                        </div>
                        <div className="flex items-center text-gray-500 text-sm">
                          <FiClock className="mr-1" />
                          {dayjs(item?.dateReview).format("M/D/YYYY")}
                        </div>
                      </div>

                      <Rate disabled allowHalf defaultValue={item?.rating} />

                      <Title level={5} className="mt-2 mb-1">
                        {item?.title}
                      </Title>

                      <Paragraph className="text-gray-700">
                        {item?.content}
                      </Paragraph>
                    </List.Item>
                  )}
                />
              </Card>
            )}
          </Row>
        </Card>
      </div>
    </>
  );
};

export default BookDetail;
