export interface LoginResponse {
  success: boolean;
  accessToken: string;
  errorMessage: string | null;
}

export interface QuestionSet {
  questionSetId: string;
  questionSetName: string;
  questionSetOrder: number;
  locked: boolean;
}

export interface Question {
  questionId: string;
  questionSetId: string;
  questionText: string;
  expectedResultText: string;
  questionOrder: number;
  minWordLength: number;
  maxWordLength: number;
}

export interface Response {
  responseId: string;
  questionId: string;
  userName: string;
  responseText: string;
  timestamp: string;
}

export interface User {
  username: string;
  pin: string;
  resetState: boolean;
}