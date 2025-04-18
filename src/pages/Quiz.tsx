import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Layout } from '../components/Layout';
import { QuestionCard } from '../components/QuestionCard';
import { Button } from '../components/Button';
import { Home, ChevronLeft, ChevronRight, Check } from 'lucide-react';
import { getQuestions, submitResponse, getQuestionSets, getUserResponses } from '../lib/api';

export const Quiz: React.FC = () => {
  const { questionSetId } = useParams<{ questionSetId: string }>();
  const navigate = useNavigate();
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState(0);
  const [answeredQuestions, setAnsweredQuestions] = useState<Set<number>>(new Set());

  const { data: questions, isLoading: loadingQuestions } = useQuery({
    queryKey: ['questions', questionSetId],
    queryFn: () => getQuestions(questionSetId!),
  });

  const { data: questionSets } = useQuery({
    queryKey: ['questionSets'],
    queryFn: getQuestionSets,
  });
  
  // Get user responses for current question
  const { data: userResponseData } = useQuery({
    queryKey: ['userResponses', questions?.[currentQuestionIndex]?.questionId],
    queryFn: () => questions?.[currentQuestionIndex] 
      ? getUserResponses(questions[currentQuestionIndex].questionId) 
      : Promise.resolve(null),
    enabled: !!questions?.[currentQuestionIndex],
  });

  // Extract actual response from the data structure
  const userResponse = userResponseData?.responses?.response?.[0]?.responseText || '';
  const hasResponse = !!userResponseData?.responses?.response?.length;

  // Reset current question index when questionset changes
  useEffect(() => {
    setCurrentQuestionIndex(0);
    setAnsweredQuestions(new Set());
  }, [questionSetId]);

  // Check if questions are already answered
  useEffect(() => {
    if (hasResponse) {
      setAnsweredQuestions(prev => new Set([...prev, currentQuestionIndex]));
    }
  }, [hasResponse, currentQuestionIndex]);

  // Check if all questions are answered
  const allQuestionsAnswered = questions && questions.length > 0 && 
    answeredQuestions.size === questions.length;

  const handleSubmit = async (response: string) => {
    if (!questions) return;

    await submitResponse(questions[currentQuestionIndex].questionId, response);
    setAnsweredQuestions(prev => new Set([...prev, currentQuestionIndex]));
    
    // Automatically navigate to next question after answering
    if (currentQuestionIndex < questions.length - 1) {
      handleNavigate('next');
    }
  };

  const handleNavigate = (direction: 'prev' | 'next') => {
    if (!questions) return;

    if (direction === 'prev' && currentQuestionIndex > 0) {
      setCurrentQuestionIndex(currentQuestionIndex - 1);
    } else if (direction === 'next' && currentQuestionIndex < questions.length - 1) {
      setCurrentQuestionIndex(currentQuestionIndex + 1);
    }
  };

  const handleDone = () => {
    // Check for next set or navigate to dashboard
    if (!questionSets) return;

    const currentSetIndex = questionSets.findIndex((set: { questionSetId: string | undefined; }) => set.questionSetId === questionSetId);
    const nextSet = questionSets
      .slice(currentSetIndex + 1)
      .find((set: { locked: any; }) => !set.locked);

    if (nextSet) {
      navigate(`/quiz/${nextSet.questionSetId}`);
    } else {
      navigate('/dashboard', { 
        state: { 
          message: 'Congratulations! You have completed all available question sets.' 
        }
      });
    }
  };

  if (loadingQuestions) {
    return (
      <Layout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500" />
        </div>
      </Layout>
    );
  }

  if (!questions || questions.length === 0) {
    return (
      <Layout>
        <div className="text-center text-red-500">
          Failed to load questions. Please try again later.
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-2xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <Button
            variant="secondary"
            onClick={() => navigate('/dashboard')}
            className="flex items-center gap-2"
          >
            <Home className="w-4 h-4" />
            Home
          </Button>
          <div className="flex items-center gap-4">
            <Button
              variant="secondary"
              onClick={() => handleNavigate('prev')}
              disabled={currentQuestionIndex === 0}
              className={`flex items-center gap-2 ${currentQuestionIndex === 0 ? 'opacity-50 cursor-not-allowed' : ''}`}
            >
              <ChevronLeft className="w-4 h-4" />
              Previous
            </Button>
            
            {allQuestionsAnswered ? (
              <Button
                variant="primary"
                onClick={handleDone}
                className="flex items-center gap-2"
              >
                <Check className="w-4 h-4" />
                Done
              </Button>
            ) : (
              <Button
                variant="secondary"
                onClick={() => handleNavigate('next')}
                disabled={currentQuestionIndex === questions.length - 1}
                className="flex items-center gap-2"
              >
                Next
                <ChevronRight className="w-4 h-4" />
              </Button>
            )}
          </div>
        </div>

        <div className="mb-6">
          <div className="flex items-center justify-between text-sm text-gray-400 mb-2">
            <span>Progress: {answeredQuestions.size} of {questions.length} questions completed</span>
            <span>Question {currentQuestionIndex + 1} of {questions.length}</span>
          </div>
          <div className="h-2 bg-gray-700 rounded-full">
            <div
              className="h-2 bg-blue-500 rounded-full transition-all duration-300"
              style={{ width: `${(answeredQuestions.size / questions.length) * 100}%` }}
            />
          </div>
        </div>

        {questions[currentQuestionIndex] && (
          <QuestionCard
            question={questions[currentQuestionIndex]}
            onSubmit={handleSubmit}
            currentNumber={currentQuestionIndex + 1}
            totalQuestions={questions.length}
            isAnswered={answeredQuestions.has(currentQuestionIndex)}
            previousResponse={userResponse}
          />
        )}
      </div>
    </Layout>
  );
};