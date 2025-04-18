import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { Layout } from '../components/Layout';
import { QuestionSetCard } from '../components/QuestionSetCard';
import { getQuestionSets } from '../lib/api';

export const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const { data: questionSets, isLoading, error } = useQuery({
    queryKey: ['questionSets'],
    queryFn: getQuestionSets,
  });

  if (isLoading) {
    return (
      <Layout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500" />
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <div className="text-center text-red-500">
          Failed to load question sets. Please try again later.
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <h2 className="text-2xl font-bold text-white mb-6">Available Question Sets</h2>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {questionSets?.map((questionSet) => (
          <QuestionSetCard
            key={questionSet.questionSetId}
            questionSet={questionSet}
            onClick={() => navigate(`/quiz/${questionSet.questionSetId}`)}
          />
        ))}
      </div>
    </Layout>
  );
};