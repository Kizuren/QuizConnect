import React, { useState, useEffect } from 'react';
import { Check, X } from 'lucide-react';
import { Button } from './Button';
import { Question } from '../types/api';

interface QuestionCardProps {
  question: Question;
  onSubmit: (response: string) => Promise<void>;
  currentNumber: number;
  totalQuestions: number;
  isAnswered?: boolean;
  previousResponse?: string;
}

export const QuestionCard: React.FC<QuestionCardProps> = ({
  question,
  onSubmit,
  currentNumber,
  totalQuestions,
  isAnswered = false,
  previousResponse = '',
}) => {
  const [response, setResponse] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Set response field to the previous response whenever the question changes or when the previous response or isAnswered status changes
  useEffect(() => {
    if (isAnswered && previousResponse) {
      setResponse(previousResponse);
    } else if (!isAnswered) {
      setResponse('');
    }
  }, [question.questionId, isAnswered, previousResponse]);

  const handleSubmit = async () => {
    if (!response.trim()) {
      setError('Please enter your response');
      return;
    }

    const wordCount = response.trim().split(/\s+/).length;
    if (wordCount < question.minWordLength || wordCount > question.maxWordLength) {
      setError(`Response must be between ${question.minWordLength} and ${question.maxWordLength} words`);
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      await onSubmit(response);
    } catch (err) {
      setError('Failed to submit response. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="bg-gray-800 rounded-lg p-6 shadow-xl">
      <h2 className="text-xl font-semibold text-white mb-4">{question.questionText}</h2>

      <div className="mb-6">
        <textarea
          value={response}
          onChange={(e) => setResponse(e.target.value)}
          disabled={isAnswered}
          className="w-full h-32 bg-gray-700 border-2 border-gray-600 rounded-lg p-3 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          placeholder={isAnswered ? 'Question already answered' : 'Type your response here...'}
        />
        <div className="mt-2 flex items-center justify-between text-sm">
          <span className="text-gray-400">
            Word count: {response.trim().split(/\s+/).filter(Boolean).length}
          </span>
          <span className="text-gray-400">
            Required: {question.minWordLength}-{question.maxWordLength} words
          </span>
        </div>
      </div>

      {error && (
        <div className="mb-4 flex items-center gap-2 text-red-500">
          <X className="w-4 h-4" />
          <span>{error}</span>
        </div>
      )}

      <Button
        onClick={handleSubmit}
        disabled={isSubmitting || isAnswered}
        className="w-full flex items-center justify-center gap-2"
      >
        <Check className="w-4 h-4" />
        {isAnswered ? 'Already Submitted' : 'Submit Response'}
      </Button>
    </div>
  );
};