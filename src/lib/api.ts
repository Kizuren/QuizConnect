import axios from 'axios';
import { useAuthStore } from '../store/auth';

const api = axios.create({
  baseURL: 'https://quizconnect.marcus7i.net/api',
});

api.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      useAuthStore.getState().logout();
    }
    return Promise.reject(error);
  }
);

// Auth endpoints
export const login = async (pin: string) => {
  const response = await api.post('/user/login', { pin });
  return response.data;
};

export const adminLogin = async (loginId: string) => {
  const response = await api.post('/admin/login', { loginId });
  return response.data;
};

// Admin endpoints
export const getAdminQuestionSets = async () => {
  const response = await api.get('/admin/questionsets');
  return response.data.questionSets.questionSets;
};

export const getAdminUsers = async () => {
  const response = await api.get('/admin/users');
  return response.data.users.users;
};

export const getAdminQuestions = async (questionSetId: string) => {
  const response = await api.get(`/admin/questions?questionSetId=${questionSetId}`);
  return response.data.questions.questions;
};

export const getAdminResponses = async (questionId: string) => {
  const response = await api.get(`/admin/responses?questionId=${questionId}`);
  return response.data.responses.response || [];
};

export const createUser = async (userName: string) => {
  const response = await api.post('/admin/users', { userName });
  return response.data;
};

export const updateUser = async (userName: string, newUserName: string) => {
  const response = await api.put('/admin/users', { userName, newUserName });
  return response.data;
};

export const resetUser = async (userName: string) => {
  const response = await api.put('/admin/users/reset', { userName });
  return response.data;
};

export const deleteUser = async (userName: string) => {
  const response = await api.delete('/admin/users', { data: { userName } });
  return response.data;
};

export const createQuestionSet = async (questionSetName: string) => {
  const response = await api.post('/admin/questionsets', { questionSetName });
  return response.data;
};

export const updateQuestionSetName = async (questionSetId: string, newQuestionSetName: string) => {
  const response = await api.put('/admin/questionsets/name', { questionSetId, newQuestionSetName });
  return response.data;
};

export const updateQuestionSetOrder = async (questionSetId: string, questionSetOrder: number) => {
  const response = await api.put('/admin/questionsets/order', { questionSetId, questionSetOrder });
  return response.data;
};

export const toggleQuestionSetLock = async (questionSetId: string, locked: boolean) => {
  const response = await api.put('/admin/questionsets/lock', { questionSetId, locked });
  return response.data;
};

export const deleteQuestionSet = async (questionSetId: string) => {
  const response = await api.delete('/admin/questionsets', { data: { questionSetId } });
  return response.data;
};

export const createQuestion = async (
  questionSetId: string,
  questionText: string,
  expectedResultText: string,
  questionOrder: number,
  minWordLength: number,
  maxWordLength: number
) => {
  const response = await api.post('/admin/questions', {
    questionSetId,
    questionText,
    expectedResultText,
    questionOrder,
    minWordLength,
    maxWordLength,
  });
  return response.data;
};

export const updateQuestion = async (
  questionId: string,
  questionText: string,
  expectedResultText: string,
  questionOrder: number,
  minWordLength: number,
  maxWordLength: number
) => {
  const response = await api.put('/admin/questions', {
    questionId,
    questionText,
    expectedResultText,
    questionOrder,
    minWordLength,
    maxWordLength,
  });
  return response.data;
};

export const deleteQuestion = async (questionId: string) => {
  const response = await api.delete('/admin/questions', { data: { questionId } });
  return response.data;
};

// User endpoints
export const getQuestionSets = async () => {
  const response = await api.get('/user/questionsets');
  return response.data.questionSets.questionSets;
};

export const getQuestions = async (questionSetId: string) => {
  const response = await api.get(`/user/questions?questionSetId=${questionSetId}`);
  return response.data.questions.questions;
};

export const submitResponse = async (questionId: string, responseText: string) => {
  const response = await api.post('/user/responses', { questionId, responseText });
  return response.data;
};

export const getUserResponses = async (questionId: string) => {
  const response = await api.get(`/user/responses?questionId=${questionId}`);
  return response.data || [];
};

export const checkUserResetState = async () => {
  const response = await api.get('/user/state');
  return response.data.resetState;
};

export default api;