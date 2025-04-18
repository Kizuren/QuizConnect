import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Layout } from '../components/Layout';
import { Button } from '../components/Button';
import { Plus, Edit2, Trash2, Lock, Unlock, Users, ChevronDown, ChevronUp, Eye, X } from 'lucide-react';
import { QuestionForm } from '../components/QuestionForm';
import {
  getAdminQuestionSets,
  getAdminUsers,
  getAdminQuestions,
  getAdminResponses,
  createQuestionSet,
  updateQuestionSetName,
  updateQuestionSetOrder,
  toggleQuestionSetLock,
  deleteQuestionSet,
  createUser,
  updateUser,
  deleteUser,
  resetUser,
  createQuestion,
  updateQuestion,
  deleteQuestion,
} from '../lib/api';

export const AdminDashboard: React.FC = () => {
  const queryClient = useQueryClient();
  const [newQuestionSetName, setNewQuestionSetName] = useState('');
  const [newUserName, setNewUserName] = useState('');
  const [showUserModal, setShowUserModal] = useState(false);
  const [selectedQuestionSet, setSelectedQuestionSet] = useState<string | null>(null);
  const [showResponsesModal, setShowResponsesModal] = useState(false);
  const [selectedQuestion, setSelectedQuestion] = useState<string | null>(null);

  const [showQuestionModal, setShowQuestionModal] = useState(false);
  const [currentQuestionSetId, setCurrentQuestionSetId] = useState<string | null>(null);
  const [editingQuestion, setEditingQuestion] = useState<any | null>(null);
  const [showQuestionSetModal, setShowQuestionSetModal] = useState(false);
  const [editingQuestionSet, setEditingQuestionSet] = useState<any | null>(null);

  const [editingUserId, setEditingUserId] = useState<string | null>(null);
  const [editedUserName, setEditedUserName] = useState('');

  const { data: questionSets, isLoading: loadingQuestionSets } = useQuery({
    queryKey: ['adminQuestionSets'],
    queryFn: getAdminQuestionSets,
  });

  const { data: users, isLoading: loadingUsers } = useQuery({
    queryKey: ['adminUsers'],
    queryFn: getAdminUsers,
  });

  const { data: questions } = useQuery({
    queryKey: ['adminQuestions', selectedQuestionSet],
    queryFn: () => selectedQuestionSet ? getAdminQuestions(selectedQuestionSet) : Promise.resolve(null),
    enabled: !!selectedQuestionSet,
  });

  const { data: responses } = useQuery({
    queryKey: ['adminResponses', selectedQuestion],
    queryFn: () => selectedQuestion ? getAdminResponses(selectedQuestion) : Promise.resolve(null),
    enabled: !!selectedQuestion,
  });

  const createQuestionSetMutation = useMutation({
    mutationFn: createQuestionSet,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestionSets'] });
      setNewQuestionSetName('');
    },
  });

  const updateQuestionSetNameMutation = useMutation({
    mutationFn: ({ questionSetId, newName }: { questionSetId: string; newName: string }) =>
      updateQuestionSetName(questionSetId, newName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestionSets'] });
    },
  });

  const updateQuestionSetOrderMutation = useMutation({
    mutationFn: ({ questionSetId, questionSetOrder }: { questionSetId: string; questionSetOrder: number }) =>
      updateQuestionSetOrder(questionSetId, questionSetOrder),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestionSets'] });
    },
  });

  const toggleLockMutation = useMutation({
    mutationFn: ({ id, locked }: { id: string; locked: boolean }) =>
      toggleQuestionSetLock(id, locked),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestionSets'] });
    },
  });

  const deleteQuestionSetMutation = useMutation({
    mutationFn: deleteQuestionSet,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestionSets'] });
    },
  });

  const createQuestionMutation = useMutation({
    mutationFn: (data: {
      questionSetId: string;
      questionText: string;
      expectedResultText: string;
      questionOrder: number;
      minWordLength: number;
      maxWordLength: number;
    }) => createQuestion(
      data.questionSetId,
      data.questionText,
      data.expectedResultText,
      data.questionOrder,
      data.minWordLength,
      data.maxWordLength
    ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestions', selectedQuestionSet] });
    },
  });

  const updateQuestionMutation = useMutation({
    mutationFn: (data: {
      questionId: string;
      questionText: string;
      expectedResultText: string;
      questionOrder: number;
      minWordLength: number;
      maxWordLength: number;
    }) => updateQuestion(
      data.questionId,
      data.questionText,
      data.expectedResultText,
      data.questionOrder,
      data.minWordLength,
      data.maxWordLength
    ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestions', selectedQuestionSet] });
    },
  });

  const deleteQuestionMutation = useMutation({
    mutationFn: deleteQuestion,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminQuestions', selectedQuestionSet] });
    },
  });

  const createUserMutation = useMutation({
    mutationFn: createUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminUsers'] });
      setNewUserName('');
    },
  });

  const updateUserMutation = useMutation({
    mutationFn: ({ userName, newUserName }: { userName: string; newUserName: string }) =>
      updateUser(userName, newUserName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminUsers'] });
    },
  });

  const deleteUserMutation = useMutation({
    mutationFn: deleteUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminUsers'] });
    },
  });

  const resetUserMutation = useMutation({
    mutationFn: resetUser,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['adminUsers'] });
    },
  });

  const handleAddQuestion = (questionSetId: string) => {
    setCurrentQuestionSetId(questionSetId);
    setEditingQuestion(null);
    setShowQuestionModal(true);
  };

  const handleEditQuestion = (question: any) => {
    setEditingQuestion(question);
    setCurrentQuestionSetId(question.questionSetId);
    setShowQuestionModal(true);
  };

  const handleQuestionFormSubmit = (data: {
    questionText: string;
    expectedResultText: string;
    minWordLength: number;
    maxWordLength: number;
  }) => {
    if (editingQuestion) {
      updateQuestionMutation.mutate({
        questionId: editingQuestion.questionId,
        questionText: data.questionText,
        expectedResultText: data.expectedResultText,
        questionOrder: editingQuestion.questionOrder,
        minWordLength: data.minWordLength,
        maxWordLength: data.maxWordLength,
      });
    } else if (currentQuestionSetId) {
      createQuestionMutation.mutate({
        questionSetId: currentQuestionSetId,
        questionText: data.questionText,
        expectedResultText: data.expectedResultText,
        questionOrder: questions?.length || 0,
        minWordLength: data.minWordLength,
        maxWordLength: data.maxWordLength,
      });
    }

    setShowQuestionModal(false);
  };

  const handleEditQuestionSet = (set: any) => {
    setEditingQuestionSet(set);
    setShowQuestionSetModal(true);
  };

  const handleQuestionSetFormSubmit = (data: {
    questionSetName: string;
    questionSetOrder: number;
  }) => {
    if (editingQuestionSet) {
      if (data.questionSetName !== editingQuestionSet.questionSetName) {
        updateQuestionSetNameMutation.mutate({
          questionSetId: editingQuestionSet.questionSetId,
          newName: data.questionSetName,
        });
      }

      if (data.questionSetOrder !== editingQuestionSet.questionSetOrder) {
        updateQuestionSetOrderMutation.mutate({
          questionSetId: editingQuestionSet.questionSetId,
          questionSetOrder: data.questionSetOrder,
        });
      }
    }

    setShowQuestionSetModal(false);
  };

  if (loadingQuestionSets || loadingUsers) {
    return (
      <Layout>
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500" />
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-8">
        <div>
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-2xl font-bold text-white">Question Sets</h2>
            <Button
              onClick={() => setShowUserModal(true)}
              className="flex items-center gap-2"
            >
              <Users className="w-4 h-4" />
              Manage Users
            </Button>
          </div>

          <div className="flex gap-4 mb-6">
            <input
              type="text"
              value={newQuestionSetName}
              onChange={(e) => setNewQuestionSetName(e.target.value)}
              placeholder="New question set name"
              className="flex-1 bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white placeholder-gray-400"
            />
            <Button
              onClick={() => createQuestionSetMutation.mutate(newQuestionSetName)}
              disabled={!newQuestionSetName.trim()}
              className="flex items-center gap-2"
            >
              <Plus className="w-4 h-4" />
              Add Set
            </Button>
          </div>

          <div className="grid gap-4">
            {questionSets?.map((set) => (
              <div
                key={set.questionSetId}
                className="bg-gray-800 rounded-lg overflow-hidden"
              >
                <div className="p-4 flex items-center justify-between">
                  <div>
                    <h3 className="text-lg font-semibold text-white">
                      {set.questionSetName}
                    </h3>
                    <div className="flex items-center gap-4 mt-1">
                      <div className="flex items-center gap-2">
                        <Button
                          variant="secondary"
                          size="sm"
                          onClick={() => handleEditQuestionSet(set)}
                        >
                          Order: {set.questionSetOrder}
                        </Button>
                      </div>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() =>
                        toggleLockMutation.mutate({
                          id: set.questionSetId,
                          locked: !set.locked,
                        })
                      }
                    >
                      {set.locked ? (
                        <Lock className="w-4 h-4" />
                      ) : (
                        <Unlock className="w-4 h-4" />
                      )}
                    </Button>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => handleEditQuestionSet(set)}
                    >
                      <Edit2 className="w-4 h-4" />
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() =>
                        deleteQuestionSetMutation.mutate(set.questionSetId)
                      }
                    >
                      <Trash2 className="w-4 h-4" />
                    </Button>
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => setSelectedQuestionSet(selectedQuestionSet === set.questionSetId ? null : set.questionSetId)}
                    >
                      {selectedQuestionSet === set.questionSetId ? (
                        <ChevronUp className="w-4 h-4" />
                      ) : (
                        <ChevronDown className="w-4 h-4" />
                      )}
                    </Button>
                  </div>
                </div>

                {selectedQuestionSet === set.questionSetId && (
                  <div className="border-t border-gray-700 p-4">
                    <div className="flex justify-between items-center mb-4">
                      <h4 className="text-lg font-semibold text-white">Questions</h4>
                      <Button
                        size="sm"
                        onClick={() => handleAddQuestion(set.questionSetId)}
                        className="flex items-center gap-2"
                      >
                        <Plus className="w-4 h-4" />
                        Add Question
                      </Button>
                    </div>
                    <div className="space-y-4">
                      {questions?.map((question) => (
                        <div
                          key={question.questionId}
                          className="bg-gray-700 p-4 rounded-lg"
                        >
                          <div className="flex justify-between items-start">
                            <div>
                              <p className="text-white font-medium mb-2">{question.questionText}</p>
                              <p className="text-gray-400 text-sm">
                                Word limit: {question.minWordLength}-{question.maxWordLength}
                              </p>
                              <p className="text-gray-400 text-sm mt-1">
                                Expected result: {question.expectedResultText}
                              </p>
                            </div>
                            <div className="flex items-center gap-2">
                              <Button
                                variant="secondary"
                                size="sm"
                                onClick={() => {
                                  setSelectedQuestion(question.questionId);
                                  setShowResponsesModal(true);
                                }}
                              >
                                <Eye className="w-4 h-4" />
                              </Button>
                              <Button
                                variant="secondary"
                                size="sm"
                                onClick={() => handleEditQuestion(question)}
                              >
                                <Edit2 className="w-4 h-4" />
                              </Button>
                              <Button
                                variant="danger"
                                size="sm"
                                onClick={() => deleteQuestionMutation.mutate(question.questionId)}
                              >
                                <Trash2 className="w-4 h-4" />
                              </Button>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>

        {showUserModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-gray-800 rounded-lg p-6 w-full max-w-2xl">
              <h2 className="text-xl font-bold text-white mb-4">User Management</h2>

              <div className="flex gap-4 mb-6">
                <input
                  type="text"
                  value={newUserName}
                  onChange={(e) => setNewUserName(e.target.value)}
                  placeholder="New username"
                  className="flex-1 bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white placeholder-gray-400"
                />
                <Button
                  onClick={() => createUserMutation.mutate(newUserName)}
                  disabled={!newUserName.trim()}
                  className="flex items-center gap-2"
                >
                  <Plus className="w-4 h-4" />
                  Add User
                </Button>
              </div>

              <div className="space-y-4 max-h-96 overflow-y-auto">
                {users?.map((user) => (
                  <div
                    key={user.username}
                    className="bg-gray-700 p-4 rounded-lg flex items-center justify-between"
                  >
                    <div>
                      {editingUserId === user.username ? (
                        <input
                          type="text"
                          value={editedUserName}
                          onChange={(e) => setEditedUserName(e.target.value)}
                          onBlur={() => {
                            if (editedUserName.trim() !== '' && editedUserName !== user.username) {
                              updateUserMutation.mutate({
                                userName: user.username,
                                newUserName: editedUserName.trim()
                              });
                            }
                            setEditingUserId(null);
                          }}
                          onKeyDown={(e) => {
                            if (e.key === 'Enter') {
                              e.currentTarget.blur();
                            } else if (e.key === 'Escape') {
                              setEditingUserId(null);
                            }
                          }}
                          autoFocus
                          className="bg-transparent border-b border-blue-500 font-semibold text-white focus:outline-none"
                        />
                      ) : (
                        <p
                          className="font-semibold text-white cursor-pointer hover:text-blue-400"
                          onClick={() => {
                            setEditingUserId(user.username);
                            setEditedUserName(user.username);
                          }}
                        >
                          {user.username}
                        </p>
                      )}
                      <p className="text-sm text-gray-400">PIN: {user.pin}</p>
                    </div>
                    <div className="flex items-center gap-2">
                      <Button
                        variant="secondary"
                        size="sm"
                        onClick={() => resetUserMutation.mutate(user.username)}
                      >
                        Reset
                      </Button>
                      <Button
                        variant="danger"
                        size="sm"
                        onClick={() => deleteUserMutation.mutate(user.username)}
                      >
                        <Trash2 className="w-4 h-4" />
                      </Button>
                    </div>
                  </div>
                ))}
              </div>

              <div className="mt-6 flex justify-end">
                <Button
                  variant="secondary"
                  onClick={() => setShowUserModal(false)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        )}

        {showResponsesModal && selectedQuestion && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-gray-800 rounded-lg p-6 w-full max-w-4xl">
              <h2 className="text-xl font-bold text-white mb-4">Student Responses</h2>

              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr>
                      <th className="text-left p-2 text-gray-400">Student</th>
                      <th className="text-left p-2 text-gray-400">Response</th>
                      <th className="text-left p-2 text-gray-400">Timestamp</th>
                    </tr>
                  </thead>
                  <tbody>
                    {responses?.map((response) => (
                      <tr key={response.responseId} className="border-t border-gray-700">
                        <td className="p-2 text-white">{response.userName}</td>
                        <td className="p-2 text-white">{response.responseText}</td>
                        <td className="p-2 text-gray-400">
                          {new Date(response.responseTime).toLocaleString()}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="mt-6 flex justify-end">
                <Button
                  variant="secondary"
                  onClick={() => {
                    setShowResponsesModal(false);
                    setSelectedQuestion(null);
                  }}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        )}

        {showQuestionModal && (
          <QuestionForm
            onSubmit={handleQuestionFormSubmit}
            onCancel={() => setShowQuestionModal(false)}
            initialData={editingQuestion ? {
              questionText: editingQuestion.questionText,
              expectedResultText: editingQuestion.expectedResultText,
              minWordLength: editingQuestion.minWordLength,
              maxWordLength: editingQuestion.maxWordLength
            } : undefined}
          />
        )}

        {showQuestionSetModal && editingQuestionSet && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-gray-800 rounded-lg p-6 w-full max-w-md relative z-10">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-xl font-bold text-white">
                  Edit Question Set
                </h2>
                <Button variant="secondary" size="sm" onClick={() => setShowQuestionSetModal(false)}>
                  <X className="w-4 h-4" />
                </Button>
              </div>

              <form onSubmit={(e) => {
                e.preventDefault();
                const formData = new FormData(e.currentTarget);
                handleQuestionSetFormSubmit({
                  questionSetName: formData.get('questionSetName') as string,
                  questionSetOrder: parseInt(formData.get('questionSetOrder') as string)
                });
              }} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">
                    Question Set Name
                  </label>
                  <input
                    type="text"
                    name="questionSetName"
                    defaultValue={editingQuestionSet.questionSetName}
                    className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-300 mb-2">
                    Order
                  </label>
                  <input
                    type="number"
                    name="questionSetOrder"
                    defaultValue={editingQuestionSet.questionSetOrder}
                    min="1"
                    className="w-full bg-gray-700 border border-gray-600 rounded-lg px-4 py-2 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors"
                    required
                  />
                </div>

                <div className="flex justify-end gap-4 mt-6">
                  <Button variant="secondary" onClick={() => setShowQuestionSetModal(false)}>
                    Cancel
                  </Button>
                  <Button type="submit">
                    Save Changes
                  </Button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </Layout>
  );
};